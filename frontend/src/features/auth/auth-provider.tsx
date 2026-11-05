"use client";

import {
  createContext,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import {
  browserLocalPersistence,
  browserSessionPersistence,
  GoogleAuthProvider,
  onAuthStateChanged,
  setPersistence,
  signInWithEmailAndPassword,
  signInWithPopup,
  signOut as firebaseSignOut,
  type Auth,
  type User,
} from "firebase/auth";
import { getFirebaseAuth } from "@/lib/firebase/client";

export type AuthStatus = "loading" | "signedIn" | "signedOut";

type AuthContextValue = {
  status: AuthStatus;
  user: User | null;
  /** Mensaje cuando falta la configuración de Firebase; null si está todo bien. */
  configError: string | null;
  /** `remember`: true mantiene la sesión abierta en este dispositivo; false la cierra al cerrar el navegador. */
  signIn: (email: string, password: string, remember: boolean) => Promise<void>;
  signInWithGoogle: (remember: boolean) => Promise<void>;
  signOut: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | null>(null);

type Boot = { auth: Auth; error: null } | { auth: null; error: string };

function boot(): Boot | null {
  if (typeof window === "undefined") return null;
  try {
    return { auth: getFirebaseAuth(), error: null };
  } catch (error) {
    return { auth: null, error: error instanceof Error ? error.message : String(error) };
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [initial] = useState(boot);
  const [session, setSession] = useState<{ status: AuthStatus; user: User | null }>({
    status: "loading",
    user: null,
  });

  const auth = initial?.auth ?? null;
  const configError = initial?.error ?? null;

  useEffect(() => {
    if (!auth) return;
    return onAuthStateChanged(auth, (user) => {
      setSession({ status: user ? "signedIn" : "signedOut", user });
    });
  }, [auth]);

  const value = useMemo<AuthContextValue>(
    () => ({
      status: configError ? "signedOut" : session.status,
      user: session.user,
      configError,
      signIn: async (email, password, remember) => {
        if (!auth) throw new Error(configError ?? "Firebase no está disponible.");
        await setPersistence(auth, remember ? browserLocalPersistence : browserSessionPersistence);
        await signInWithEmailAndPassword(auth, email, password);
      },
      signInWithGoogle: async (remember) => {
        if (!auth) throw new Error(configError ?? "Firebase no está disponible.");
        await setPersistence(auth, remember ? browserLocalPersistence : browserSessionPersistence);
        await signInWithPopup(auth, new GoogleAuthProvider());
      },
      signOut: async () => {
        if (auth) await firebaseSignOut(auth);
      },
    }),
    [auth, configError, session],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth debe usarse dentro de <AuthProvider>.");
  return context;
}
