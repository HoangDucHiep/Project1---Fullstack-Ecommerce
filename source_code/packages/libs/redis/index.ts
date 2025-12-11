import Redis from "ioredis";

const redis = new Redis(process.env.REDIS_DATABASE_URI!);

export default redis;
