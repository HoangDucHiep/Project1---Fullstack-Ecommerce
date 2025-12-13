import express from 'express';
import cors from 'cors';
import cookieParser from 'cookie-parser';
import swaggerUi from 'swagger-ui-express';
import swaggerDocument from './swagger-output.json';
import { errorMiddleware } from '@packages/error-handler/error-middleware';
import router from './routes/product.routes';

const app = express();

// Middleware
// Enable CORS
app.use(
  cors({
    origin: true, // Allow all origins in development
    allowedHeaders: ["Content-Type", "Authorization", "Accept"],
    methods: ["GET", "POST", "PUT", "DELETE", "OPTIONS"],
    credentials: true,
  })
);
app.use(express.json({ limit: '50mb' })); // Body Parsing with increased limit for image uploads
app.use(express.urlencoded({ limit: '50mb', extended: true }));
app.use(cookieParser()); // for parsing application/x-www-form-urlencoded

app.get('/', (req, res) => {
    res.send({ 'message': 'Welcome to product-service!' });
});

// Routes
app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(swaggerDocument));
app.get("/docs-json", (req, res) => {
    res.json(swaggerDocument);
})

app.use("/api", router);


// Error Handling Middleware
app.use(errorMiddleware);


const port = process.env.PORT || 6002;
const server = app.listen(port, () => {
    console.log(`Product service is running at http://localhost:${port}/api`);
    console.log(`Swagger docs at http://localhost:${port}/api-docs`);
});

server.on('error', (err) => {
    console.error("Server error:", err);
});

