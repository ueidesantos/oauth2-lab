# OAuth2 Lab

Laboratório de aprendizado para autenticação SSO com **Microsoft Entra ID**, **Google** e **GitHub** usando OAuth 2.0 / OpenID Connect em ASP.NET Core 10.

> ⚠️ **Este projeto é exclusivamente para estudos.** Tokens e dados de sessão são exibidos em tela para fins didáticos. Nunca exponha tokens em produção.

---

## Conceitos Abordados

### OAuth 2.0 vs OpenID Connect

| | OAuth 2.0 | OpenID Connect (OIDC) |
|---|---|---|
| **Propósito** | Autorização (acesso a recursos) | Autenticação (identidade do usuário) |
| **Token principal** | Access Token | Id Token (JWT) |
| **Quem sou eu?** | Não define | Sim, via `id_token` e UserInfo endpoint |
| **Padrão** | RFC 6749 | Camada sobre OAuth 2.0 |

### Tipos de Token

| Token | Propósito | Contém identidade? |
|---|---|---|
| **Access Token** | Acesso a APIs/recursos do provedor | Às vezes (JWT) |
| **Id Token** | Prova de autenticação do usuário | Sempre (JWT com claims OIDC) |
| **Refresh Token** | Renovar tokens sem re-autenticar | Não |

### Claims, Scopes e Roles

- **Claim**: par chave/valor sobre o usuário (ex: `email`, `name`, `sub`)
- **Scope**: permissão solicitada ao provedor (ex: `openid`, `profile`, `email`)
- **Role**: tipo especial de claim usado para controle de acesso

### Diferenças entre Provedores

| | Microsoft | Google | GitHub |
|---|---|---|---|
| **Protocolo** | OIDC completo | OIDC completo | OAuth 2.0 apenas |
| **Id Token** | ✅ JWT | ✅ JWT | ❌ não emite |
| **Refresh Token** | ✅ (`offline_access`) | ✅ (se solicitado) | ✅ |
| **Tenant** | Sim (corporativo) | Não | Não |
| **User API** | Graph API | People API | `api.github.com/user` |

---

## Estrutura do Projeto

```
src/OAuth2Lab.Web/
├── Features/
│   ├── Home/               → Tela de login com os 3 botões
│   ├── Auth/
│   │   ├── Login/          → Inicia o fluxo OAuth (Challenge)
│   │   ├── Callback/       → Recebe o code, salva sessão no MongoDB
│   │   ├── Logout/         → Encerra sessão local
│   │   └── Error/          → Exibe erros de autenticação
│   └── TokenInspector/     → Exibe payload bruto do SSO
├── Infrastructure/
│   ├── Auth/               → Configuração dos 3 provedores
│   └── Persistence/        → MongoDB (LoginSession)
├── Shared/
│   ├── Models/             → ViewModels e documentos Mongo
│   ├── Extensions/         → Helpers de Claims e JWT
│   └── Views/              → _Layout, _ViewImports, _ViewStart
└── Program.cs
```

---

## Configuração dos Provedores

### Usando User Secrets (recomendado para desenvolvimento local)

```bash
cd src/OAuth2Lab.Web
dotnet user-secrets init
```

### Microsoft Entra ID

1. Acesse o [Portal Azure](https://portal.azure.com) → Azure Active Directory → Registros de aplicativo
2. Crie um novo registro
3. Em **Redirect URI**, adicione: `https://localhost:7055/signin-microsoft`
4. Copie o **Application (client) ID** e **Directory (tenant) ID**
5. Em **Certificados e segredos**, crie um Client Secret

```bash
dotnet user-secrets set "Authentication:Microsoft:TenantId" "seu-tenant-id"
dotnet user-secrets set "Authentication:Microsoft:ClientId" "seu-client-id"
dotnet user-secrets set "Authentication:Microsoft:ClientSecret" "seu-client-secret"
```

### Google

1. Acesse o [Google Cloud Console](https://console.cloud.google.com)
2. Crie um projeto → APIs & Serviços → Credenciais → OAuth 2.0
3. Em **Origens JavaScript autorizadas**: `https://localhost:7055`
4. Em **URIs de redirecionamento autorizados**: `https://localhost:7055/signin-google`
```bash
dotnet user-secrets set "Authentication:Google:ClientId" "seu-client-id.apps.googleusercontent.com"
dotnet user-secrets set "Authentication:Google:ClientSecret" "seu-client-secret"
```

### GitHub

1. Acesse [GitHub → Settings → Developer settings → OAuth Apps](https://github.com/settings/developers)
2. Crie uma nova OAuth App
3. Em **Authorization callback URL**: `https://localhost:7000/signin-github`

```bash
dotnet user-secrets set "Authentication:GitHub:ClientId" "seu-client-id"
dotnet user-secrets set "Authentication:GitHub:ClientSecret" "seu-client-secret"
```

### MongoDB

```bash
dotnet user-secrets set "MongoDB:ConnectionString" "mongodb+srv://usuario:senha@cluster.mongodb.net/"
dotnet user-secrets set "MongoDB:DatabaseName" "oauth2lab"
```

---

## Como Rodar Localmente

```bash
# Clone e entre no diretório
git clone https://github.com/ueidesantos/oauth2-lab.git
cd oauth2-lab

# Configure os secrets (veja seção acima)
cd src/OAuth2Lab.Web
dotnet user-secrets set "..." "..."

# Execute
dotnet run
```

A aplicação estará disponível em `https://localhost:7055` (ou a porta configurada no launchSettings.json).

---

## Como Testar

1. Acesse `https://localhost:7000`
2. Clique em um dos botões de login
3. Autentique-se no provedor escolhido
4. Você será redirecionado para `/token-inspector` com:
   - Dados do usuário (nome, email, avatar)
   - Claims recebidas pelo provedor
   - Access Token, Id Token e Refresh Token (quando disponíveis)
   - Payload JWT decodificado
   - Escopos concedidos

---

## Problemas Comuns

| Problema | Causa provável | Solução |
|---|---|---|
| `redirect_uri_mismatch` | URI de callback diferente da configurada | Verifique se a porta e o path batem exatamente |
| `invalid_client` | ClientId ou ClientSecret errados | Revise os User Secrets |
| GitHub sem Id Token | GitHub não é OIDC | Esperado — o lab exibirá apenas Access Token |
| Cookie não persiste | HTTPS desabilitado | Rode sempre com HTTPS em desenvolvimento |
| MongoDB connection fail | Connection string inválida | Verifique IP whitelist no Atlas (0.0.0.0/0 para dev) |

---

## O que NÃO fazer em produção

- ❌ Exibir tokens em tela
- ❌ Armazenar tokens completos no banco sem criptografia
- ❌ Usar `offline_access` sem necessidade real de refresh
- ❌ Deixar `SaveTokens = true` em app que não precisa dos tokens
- ❌ Usar `SameSite=Lax` em contextos de iframe/cross-origin
- ❌ Expor a rota `/token-inspector` para usuários finais

---

## Próximos Passos

- [ ] Adicionar autorização por roles/claims (ex: apenas `@empresa.com`)
- [ ] Implementar PKCE explícito para maior segurança no fluxo Authorization Code
- [ ] Adicionar logout federado (Single Logout / Front-Channel Logout)
- [ ] Comparar tokens de diferentes provedores em uma única view
- [ ] Adicionar suporte a Azure AD B2C como provedor adicional
- [ ] Implementar token refresh automático via background service
