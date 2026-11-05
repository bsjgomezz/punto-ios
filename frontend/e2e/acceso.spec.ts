import { expect, test } from "@playwright/test";

test("sin sesión, /panel redirige a /ingresar", async ({ page }) => {
  await page.goto("/panel");

  await expect(page).toHaveURL(/\/ingresar$/);
  await expect(page.getByRole("heading", { level: 1, name: "Ingresar" })).toBeVisible();
});

test("el formulario pide completar los campos", async ({ page }) => {
  await page.goto("/ingresar");

  await page.getByRole("button", { name: "Ingresar", exact: true }).click();

  await expect(page.getByRole("alert").filter({ hasText: "Ingresá tu email y tu contraseña." })).toBeVisible();
});

test("credenciales incorrectas muestran un error claro", async ({ page }) => {
  // Se simula la respuesta de Firebase para que la prueba no dependa de internet ni de cuentas reales.
  await page.route("**/identitytoolkit.googleapis.com/**", (route) =>
    route.fulfill({
      status: 400,
      contentType: "application/json",
      headers: {
        "access-control-allow-origin": "*",
        "access-control-allow-headers": "*",
        "access-control-allow-methods": "*",
      },
      body: JSON.stringify({
        error: {
          code: 400,
          message: "INVALID_LOGIN_CREDENTIALS",
          errors: [{ message: "INVALID_LOGIN_CREDENTIALS", domain: "global", reason: "invalid" }],
        },
      }),
    }),
  );
  await page.goto("/ingresar");

  await page.getByLabel("Email").fill("alguien@ejemplo.com");
  await page.getByLabel("Contraseña").fill("incorrecta");
  await page.getByRole("button", { name: "Ingresar", exact: true }).click();

  await expect(page.getByRole("alert").filter({ hasText: "Email o contraseña incorrectos." })).toBeVisible();
  await expect(page).toHaveURL(/\/ingresar$/);
});
