import { expect, test, type Page } from "@playwright/test";

const CORS = { "access-control-allow-origin": "*" };

async function mockApi(page: Page, ready: number) {
  await page.route("**/health", (route) =>
    route.fulfill({ status: 200, headers: CORS, body: "Healthy" }),
  );
  await page.route("**/health/ready", (route) =>
    route.fulfill({ status: ready, headers: CORS, body: ready === 200 ? "Healthy" : "Unhealthy" }),
  );
}

test("la página inicial muestra el sistema operativo", async ({ page }) => {
  await mockApi(page, 200);
  await page.goto("/");

  await expect(page.getByRole("heading", { level: 1, name: "Punto iOS" })).toBeVisible();
  const servidor = page.getByRole("listitem").filter({ hasText: "Servidor" });
  const baseDeDatos = page.getByRole("listitem").filter({ hasText: "Base de datos" });
  await expect(servidor).toContainText("Responde");
  await expect(baseDeDatos).toContainText("Conectada");
});

test("si la base falla, explica cómo resolverlo", async ({ page }) => {
  await mockApi(page, 503);
  await page.goto("/");

  const baseDeDatos = page.getByRole("listitem").filter({ hasText: "Base de datos" });
  await expect(baseDeDatos).toContainText("No disponible");
  await expect(baseDeDatos).toContainText("docker compose up -d");
});
