import LoginForm from "@/components/LoginForm";

export const dynamic = "force-dynamic";

export default function AdminLoginPage() {
  return (
    <main className="login-page">
      <div className="login-card">
        <div className="login-green-bar" />
        <div className="login-body">
          {/* eslint-disable-next-line @next/next/no-img-element */}
          <img
            src="/logo-renergeia.png"
            alt="Renergeia"
            className="login-logo"
          />
          <h1 className="login-title">Capacitaciones e Inducciones de Renergeia</h1>
          <p className="login-subtitle">
            Renergeia Colombia SAS &middot; Acceso restringido
          </p>
          <LoginForm />
          <p className="login-help">
            Si no tienes acceso, solicítalo al área de Calidad de Renergeia.
          </p>
        </div>
      </div>
    </main>
  );
}
