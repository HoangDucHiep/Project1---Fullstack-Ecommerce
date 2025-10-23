process.env.NODE_TLS_REJECT_UNAUTHORIZED = "0";

const { generate } = require("openapi-typescript-codegen");

(async () => {
    try {
        await generate({
            input: "https://localhost:5001/swagger/v1/swagger.json",
            output: "./src/api",
            httpClient: "axios",
            clientName: "ApiClient",
            useOptions: true,
            useUnionTypes: true,
        });

        console.log("✅ API client đã được sinh tự động tại src/api/");
    } catch (err) {
        console.error("❌ Lỗi sinh API client:", err);
    }
})();
