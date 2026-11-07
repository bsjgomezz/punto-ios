import { BackLink } from "@/components/back-link";
import { LoginForm } from "@/features/auth/login-form";

export default function IngresarPage() {
  return (
    <main className="shell shell-narrow">
      <p className="brand">
        <span className="brand-mark" aria-hidden="true" />
        Punto iOS
      </p>
      <h1 className="page-title">Ingresar</h1>
      <p className="hero-lead">Usá la cuenta que te dio el administrador del local.</p>

      <LoginForm />

      <BackLink href="/">Volver al inicio</BackLink>
    </main>
  );
}
