"use client";

import { useEffect, useRef, useState, useSyncExternalStore, type FormEvent } from "react";
import { useRouter } from "next/navigation";
import { PasswordInput } from "@/components/password-input";
import { describeAuthError } from "./auth-errors";
import { useAuth } from "./auth-provider";
import { readRememberedEmail, writeRememberedEmail } from "./remembered-email";

const noSubscribe = () => () => {};

export function LoginForm() {
  const { status, signIn, signInWithGoogle, configError } = useAuth();
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const formRef = useRef<HTMLFormElement>(null);
  const rememberedEmail = useSyncExternalStore(noSubscribe, readRememberedEmail, () => "");

  useEffect(() => {
    if (status === "signedIn") router.replace("/panel");
  }, [status, router]);

  async function onSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const data = new FormData(event.currentTarget);
    const email = String(data.get("email") ?? "").trim();
    const password = String(data.get("password") ?? "");
    const remember = data.get("remember") === "on";

    if (!email || !password) {
      setError("Ingresá tu email y tu contraseña.");
      return;
    }

    setError(null);
    setSubmitting(true);
    try {
      await signIn(email, password, remember);
      writeRememberedEmail(remember ? email : null);
      router.replace("/panel");
    } catch (caught) {
      setError(describeAuthError(caught));
      setSubmitting(false);
    }
  }

  async function onGoogle() {
    const remember = formRef.current?.elements.namedItem("remember") instanceof HTMLInputElement
      ? (formRef.current.elements.namedItem("remember") as HTMLInputElement).checked
      : true;
    setError(null);
    setSubmitting(true);
    try {
      await signInWithGoogle(remember);
      if (!remember) writeRememberedEmail(null);
      router.replace("/panel");
    } catch (caught) {
      setError(describeAuthError(caught));
      setSubmitting(false);
    }
  }

  return (
    <form className="board form" ref={formRef} onSubmit={onSubmit} noValidate>
      {configError ? (
        <p className="form-error" role="alert">
          {configError}
        </p>
      ) : null}

      <div className="field">
        <label htmlFor="email">Email</label>
        <input
          id="email"
          name="email"
          type="email"
          autoComplete="email"
          inputMode="email"
          key={rememberedEmail}
          defaultValue={rememberedEmail}
        />
      </div>

      <div className="field">
        <label htmlFor="password">Contraseña</label>
        <PasswordInput id="password" name="password" autoComplete="current-password" />
      </div>

      <label className="remember">
        <input type="checkbox" name="remember" defaultChecked />
        Recordarme en este dispositivo
      </label>

      {error ? (
        <p className="form-error" role="alert">
          {error}
        </p>
      ) : null}

      <button type="submit" className="btn" disabled={submitting}>
        {submitting ? "Ingresando…" : "Ingresar"}
      </button>

      <p className="form-divider">o</p>

      <button type="button" className="btn btn-quiet" onClick={onGoogle} disabled={submitting}>
        Ingresar con Google
      </button>
    </form>
  );
}
