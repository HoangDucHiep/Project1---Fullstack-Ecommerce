import prisma from "@packages/libs/prisma";
import cron from "node-cron";


console.log("✅ Product deletion cron job initialized. Will run every hour.");


cron.schedule("0 * * * *", async () => {
  try {
    const now = new Date();

    // Delete products where `deletedAt` is older than 24 hours
    const deletedProducts = await prisma.products.deleteMany({
      where: {
        isDeleted: true,
        deletedAt: { lte: now },
      },
    });
    console.log(
  `🗑️ ${deletedProducts.count} expired products permanently deleted.`
);
  } catch (error) {
    console.log(error)
  }
});
