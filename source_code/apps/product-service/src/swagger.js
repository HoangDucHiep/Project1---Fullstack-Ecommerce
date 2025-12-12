import swaggerAutogen from "swagger-autogen";

const doc = {
  info: {
    title: "Product Service API",
    description: "Auto-generated Swagger documentation for Auth Service API",
    version: "1.0.0",
  },
  host: "localhost:6002",
  schemes: ["http"],
  basePath: "/api/",
  prefix: "/api/",
};

const outputFile = "./swagger-output.json";
const endpointsFiles = ["./routes/product.routes.ts"];

swaggerAutogen()(outputFile, endpointsFiles, doc);
