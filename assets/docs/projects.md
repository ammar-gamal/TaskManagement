# Projects API

Provides endpoints for managing projects and project-related tasks.

All project endpoints require authentication.

**Base URL**

```
/api/projects
```

**Authentication**

Protected endpoints require a valid JWT token.

```http
Authorization: Bearer <jwt-token>
```

---

## Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/projects/{id}` | Retrieves a project by its identifier. |
| GET | `/api/projects` | Retrieves a paginated list of projects. |
| POST | `/api/projects` | Creates a new project. |
| PUT | `/api/projects/{id}` | Updates an existing project. |
| DELETE | `/api/projects/{id}` | Deletes a project. |
| POST | `/api/projects/{id}/tasks` | Creates a task under a project. |
| GET | `/api/projects/{id}/tasks` | Retrieves tasks belonging to a project. |

---

## Get Project by ID

Retrieves a single project using its identifier.

### Request

**GET** `/api/projects/{id}`

**Path Parameters**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | integer | Yes | Unique identifier of the project. |

**Example**

```http
GET /api/projects/1
```

### Responses

**200 OK**

```json
{
  "id": 1,
  "name": "Website Revamp",
  "description": "Redesign the marketing website.",
  "createdAt": "2026-07-20T10:00:00Z",
  "updatedAt": "2026-07-22T15:30:00Z"
}
```

**Response Properties**

| Property | Type | Description |
|----------|------|-------------|
| `id` | integer | Unique project identifier. |
| `name` | string | Project name. |
| `description` | string/null | Optional project description. |
| `createdAt` | datetime | Project creation date. |
| `updatedAt` | datetime/null | Last update date. |

**Possible Errors**

| Status Code | Description |
|-------------|-------------|
| 404 Not Found | Project does not exist. |

See [Error Responses](errors.md) for the response format.

---

## List Projects

Retrieves a paginated list of projects.

### Request

**GET** `/api/projects`

**Query Parameters**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `pageIndex` | integer | 1 | Page number to retrieve. |
| `limit` | integer | 20 | Number of projects per page. Maximum value is 100. |

**Example**

```http
GET /api/projects?pageIndex=1&limit=10
```

### Responses

**200 OK**

```json
{
  "pageItems": [
    {
      "id": 1,
      "name": "Website Revamp",
      "description": "Redesign the marketing website.",
      "createdAt": "2026-07-20T10:00:00Z",
      "updatedAt": "2026-07-22T15:30:00Z"
    }
  ],
  "totalPages": 3,
  "totalCount": 25,
  "pageIndex": 1,
  "hasNext": true,
  "hasPrevious": false
}
```

**Response Properties**

| Property | Type | Description |
|----------|------|-------------|
| `pageItems` | array | Projects included in the current page. |
| `totalPages` | integer | Total number of pages. |
| `totalCount` | integer | Total number of projects. |
| `pageIndex` | integer | Current page number. |
| `hasNext` | boolean | Indicates whether another page exists. |
| `hasPrevious` | boolean | Indicates whether a previous page exists. |

---

## Create Project

Creates a new project.

### Request

**POST** `/api/projects`

**Request Body**

```json
{
  "name": "Website Revamp",
  "description": "Redesign the marketing website."
}
```

**Request Properties**

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `name` | string | Yes | Unique project name. |
| `description` | string/null | No | Optional project description. |

### Responses

**201 Created**

```json
{
  "id": 1,
  "name": "Website Revamp",
  "description": "Redesign the marketing website.",
  "createdAt": "2026-07-24T10:00:00Z",
  "updatedAt": null
}
```

The response includes the created project resource. The `Location` header contains the URL of the created project:

```http
Location: /api/projects/1
```

**Possible Errors**

| Status Code | Description |
|-------------|-------------|
| 400 Bad Request | Invalid project data. |
| 409 Conflict | A project with the same name already exists. |

See [Error Responses](errors.md) for the response format.

---

## Update Project

Updates an existing project.

### Request

**PUT** `/api/projects/{id}`

**Path Parameters**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | integer | Yes | Unique identifier of the project. |

**Request Body**

```json
{
  "name": "Updated Website Revamp",
  "description": "Updated project description."
}
```

**Request Properties**

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `name` | string | Yes | Updated project name. |
| `description` | string/null | No | Updated project description. |

### Responses

**200 OK**

```json
{
  "id": 1,
  "name": "Updated Website Revamp",
  "description": "Updated project description.",
  "createdAt": "2026-07-20T10:00:00Z",
  "updatedAt": "2026-07-24T12:00:00Z"
}
```

**Possible Errors**

| Status Code | Description |
|-------------|-------------|
| 400 Bad Request | Invalid project data. |
| 404 Not Found | Project does not exist. |
| 409 Conflict | A project with the same name already exists. |

See [Error Responses](errors.md) for the response format.

---

## Delete Project

Deletes an existing project.

### Request

**DELETE** `/api/projects/{id}`

**Path Parameters**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | integer | Yes | Unique identifier of the project. |

### Responses

**204 No Content**

Project was deleted successfully. No response body is returned.

**Possible Errors**

| Status Code | Description |
|-------------|-------------|
| 404 Not Found | Project does not exist. |

See [Error Responses](errors.md) for the response format.

---

## Create Task for Project

Creates a new task under a specific project.

### Request

**POST** `/api/projects/{id}/tasks`

**Path Parameters**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | integer | Yes | Project identifier. |

**Request Body**

```json
{
  "title": "Implement authentication",
  "description": "Add JWT authentication support.",
  "status": "Todo",
  "priority": "High",
  "dueDate": "2026-08-01"
}
```

**Request Properties**

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `title` | string | Yes | Task title. |
| `description` | string/null | No | Optional task description. |
| `status` | string | No | Task status. |
| `priority` | string | No | Task priority. |
| `dueDate` | date/null | No | Optional task due date. |

**Status Values**

```
Todo
InProgress
Done
```

**Priority Values**

```
Low
Medium
High
```

### Responses

**201 Created**

```json
{
  "id": 1,
  "projectId": 1,
  "title": "Implement authentication",
  "description": "Add JWT authentication support.",
  "status": "Todo",
  "priority": "High",
  "dueDate": "2026-08-01",
  "createdAt": "2026-07-24T12:00:00Z",
  "updatedAt": null
}
```

**Possible Errors**

| Status Code | Description |
|-------------|-------------|
| 400 Bad Request | Invalid task data. |
| 404 Not Found | Project does not exist. |

See [Error Responses](errors.md) for the response format.

---

## List Project Tasks

Retrieves a paginated list of tasks belonging to a project.

### Request

**GET** `/api/projects/{id}/tasks`

**Path Parameters**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | integer | Yes | Project identifier. |

**Query Parameters**

| Parameter | Type | Description |
|-----------|------|-------------|
| `pageIndex` | integer | Page number to retrieve. |
| `limit` | integer | Number of tasks per page. Maximum value is 100. |
| `status` | string | Filter tasks by status. |
| `priority` | string | Filter tasks by priority. |
| `dueDateFrom` | date | Filter tasks with due date greater than or equal to this value. |
| `dueDateTo` | date | Filter tasks with due date less than or equal to this value. |
| `q` | string | Search across task titles and descriptions. |
| `sortBy` | string | Field used for sorting. |
| `sortDir` | string | Sorting direction. |

**Filter Values — Status**

```
Todo
InProgress
Done
```

**Filter Values — Priority**

```
Low
Medium
High
```

**Sort Fields**

```
DueDate
Priority
CreatedAt
```

**Sort Directions**

```
Asc
Desc
```

**Example**

```http
GET /api/projects/1/tasks?pageIndex=1&limit=10&status=Todo&sortBy=DueDate&sortDir=Asc
```

### Responses

**200 OK**

```json
{
  "pageItems": [
    {
      "id": 1,
      "projectId": 1,
      "projectName": "Website Revamp",
      "title": "Implement authentication",
      "description": "Add JWT authentication support.",
      "status": "Todo",
      "priority": "High",
      "dueDate": "2026-08-01",
      "createdAt": "2026-07-24T12:00:00Z",
      "updatedAt": null
    }
  ],
  "totalPages": 5,
  "totalCount": 45,
  "pageIndex": 1,
  "hasNext": true,
  "hasPrevious": false
}
```

**Possible Errors**

| Status Code | Description |
|-------------|-------------|
| 404 Not Found | Project does not exist. |

See [Error Responses](errors.md) for the response format.
