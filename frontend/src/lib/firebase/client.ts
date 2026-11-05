import { getApp, getApps, initializeApp } from "firebase/app";
import { getAuth, type Auth } from "firebase/auth";

/**
 * Devuelve el Auth de Firebase. Lanza un error claro si falta la configuración
 * (variables NEXT_PUBLIC_FIREBASE_* en frontend/.env.local).
 */
export function getFirebaseAuth(): Auth {
  const apiKey = process.env.NEXT_PUBLIC_FIREBASE_API_KEY;
  const authDomain = process.env.NEXT_PUBLIC_FIREBASE_AUTH_DOMAIN;
  const projectId = process.env.NEXT_PUBLIC_FIREBASE_PROJECT_ID;

  if (!apiKey || !authDomain || !projectId) {
    throw new Error(
      "Falta la configuración de Firebase. Completá NEXT_PUBLIC_FIREBASE_API_KEY, NEXT_PUBLIC_FIREBASE_AUTH_DOMAIN y NEXT_PUBLIC_FIREBASE_PROJECT_ID en frontend/.env.local.",
    );
  }

  const app = getApps().length > 0 ? getApp() : initializeApp({ apiKey, authDomain, projectId });
  return getAuth(app);
}
