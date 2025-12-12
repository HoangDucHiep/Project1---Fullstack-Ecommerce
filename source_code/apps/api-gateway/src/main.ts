import express from "express";
import cors from "cors";
import proxy from "express-http-proxy";
import morgan from "morgan";
import rateLimit, { ipKeyGenerator } from "express-rate-limit";
import swaggerUi from "swagger-ui-express";
import axios from "axios";
import cookieParser from "cookie-parser";
import initializeSiteConfig from "./libs/initializeSiteConfig";


const app = express();

// Middleware
// Enable CORS
app.use(
  cors({
    origin: ["http://localhost:3000"],
    allowedHeaders: ["Content-Type", "Authorization"],
    credentials: true,
  })
);

app.use(morgan("dev"));                                                 // Logging
app.use(express.json({ limit: "100mb" }));                              // Body Parsing
app.use(express.urlencoded({ limit: "100mb", extended: true }));        // for parsing application/x-www-form-urlencoded
app.use(cookieParser());                                                // Cookie Parsing
app.set("trust proxy", 1);                                              // Trust first proxy

// Rate Limiting
const limiter = rateLimit({
  windowMs: 15 * 60 * 1000, // 15 minutes
  max: (req: any) => (req.user ? 1000 : 100),
  message: { error: "Too many requests, please try again later." },
  standardHeaders: true,
  legacyHeaders: false,
  keyGenerator: (req: any) => req.ip
});

app.use(limiter);

app.get("/gateway-health", (req, res) => {
  res.send({ message: "Welcome to api-gateway!" });
});

// proxies
app.use("/", proxy("http://localhost:6001")); // auth-service
app.use("/product", proxy("http://localhost:6002")); // auth-service

const port = process.env.PORT || 8080;
const server = app.listen(port, () => {
  console.log(`Listening at http://localhost:${port}`);

  try {
    initializeSiteConfig()
    console.log("Site config initialized");
  } catch (error) {
    console.error("Site config initialization failed:", error);
  }
});
server.on("error", console.error);
