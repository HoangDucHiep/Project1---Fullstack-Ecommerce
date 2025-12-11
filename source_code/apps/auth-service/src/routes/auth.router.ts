import expess, { Router } from "express";
import { userRegistration } from "../controller/auth.controller";

const router:Router = expess.Router();

router.post("/user-registration", userRegistration);

export default router;
