import express from 'express';
import cors from 'cors';
import cookieParser from 'cookie-parser';
import { errorMiddleware } from '../../../packages/error-handler/error-middleware';


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
app.use(express.json()); // Body Parsing
app.use(cookieParser()); // for parsing application/x-www-form-urlencoded

app.get('/', (req, res) => {
    res.send({ 'message': 'Welcome to auth-service!' });
});

app.use(errorMiddleware);


const port = process.env.PORT || 6001;
const server = app.listen(port, () => {
    console.log(`Auth service is running at http://localhost:${port}/api`);
    console.log(`Swagger docs at http://localhost:${port}/api-docs`);
});

server.on('error', (err) => {
    console.error("Server error:", err);
});

