# Portal Web - Coleta Escolar (Astro + Vercel)

Hub e Portal Central de acesso ao ecossistema do **Sistema de Coleta de Dados Escolares**.

Construído com **[Astro](https://astro.build)** e preparado para deploy instantâneo na **[Vercel](https://vercel.com)**.

---

## 🎯 O que este Portal conecta:

1. **Aplicativo Mobile Android:**
   * Download direto do APK (`app-release.apk`) para os agentes de campo.
   * QR Code para download e instalação rápida apontando a câmera do celular.
   * Guia passo a passo de instalação no Android.

2. **Painel Administrativo Web (Web Admin):**
   * Acesso direto ao dashboard administrativo em produção na AWS (`http://18.219.242.45`).
   * Visualização das funcionalidades de censo escolar e relatórios.

3. **Backend & Nuvem AWS:**
   * Monitoramento de status em tempo real da instância AWS EC2 (`18.219.242.45`).
   * Link direto para a documentação interativa Swagger OpenAPI (`http://18.219.242.45:8080/swagger`).

---

## 🚀 Como Rodar Localmente

```bash
cd portal-web
npm install
npm run dev
```

Acesse no navegador: `http://localhost:4321`

---

## ☁️ Como Fazer o Deploy na Vercel

### Opção 1: Pelo Painel da Vercel (Recomendada via GitHub)

1. Acesse **[vercel.com](https://vercel.com)** e faça login com sua conta do GitHub.
2. Clique em **"Add New..."** ➜ **"Project"**.
3. Importe o repositório: `wendellsantos2/projeto_sistema_coleta_dadosEscolares`.
4. Em **Root Directory**, clique em **Edit** e selecione a pasta: **`portal-web`**.
5. O Framework Preset será detectado automaticamente como **Astro**.
6. Clique em **"Deploy"**!
7. Em menos de 1 minuto, a Vercel gerará uma URL pública gratuita (ex: `https://portal-coleta-escolar.vercel.app`) com HTTPS automático.

### Opção 2: Pela Linha de Comando (CLI)

```bash
cd portal-web
npx vercel
```
Siga os passos no terminal para vincular à sua conta e publicar.
