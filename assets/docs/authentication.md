# Authentication API

Provides endpoints for user registration and authentication.

**Base URL**

```
/api/auth
```

## Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/register` | Creates a new user account. |
| POST | `/login` | Authenticates a user and returns a JWT token. |

---

## Register

Creates a new user account.

### Request

**POST** `/api/auth/register`

**Request Body**

```json
{
  "username": "ammar",
  "password": "12345",
  "confirmPassword": "12345"
}
```

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `username` | string | Yes | Unique username for the account. |
| `password` | string | Yes | Account password. |
| `confirmPassword` | string | Yes | Must match the provided password. |

### Responses

**201 Created**

User account was created successfully. No response body is returned.

**Possible Errors**

| Status Code | Description |
|-------------|-------------|
| 400 Bad Request | Invalid request data. |
| 409 Conflict | A user with the provided username already exists. |

See [Error Responses](errors.md) for the response format.

---

## Login

Authenticates a user and returns a JWT access token.

### Request

**POST** `/api/auth/login`

**Request Body**

```json
{
  "username": "ammar",
  "password": "12345"
}
```

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `username` | string | Yes | Username of the account. |
| `password` | string | Yes | Account password. |

### Responses

**200 OK**

Authentication succeeded.

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "tokenExpiration": "2026-07-24T18:30:00Z"
}
```

**Response Properties**

| Property | Type | Description |
|----------|------|-------------|
| `token` | string | JWT access token used to access protected endpoints. |
| `tokenExpiration` | datetime | UTC date and time when the token expires. |

**Possible Errors**

| Status Code | Description |
|-------------|-------------|
| 400 Bad Request | Invalid request data. |
| 401 Unauthorized | Invalid username or password. |

See [Error Responses](errors.md) for the response format.

---

## Using the JWT Token

After successful authentication, include the token in the `Authorization` header when calling protected endpoints.

**Request Header**

```http
Authorization: Bearer <jwt-token>
```

**Example**

```http
GET /api/projects
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```
