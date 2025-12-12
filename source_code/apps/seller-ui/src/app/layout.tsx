import "./global.css";
import Provider from "./provider";

export const metadata = {
  title: "EShop - Seller",
  description: "EShop - Seller",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body>
        <Provider>{children}</Provider>
      </body>
    </html>
  );
}
