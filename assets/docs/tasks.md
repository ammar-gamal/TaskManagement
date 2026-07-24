# Tasks API

Provides endpoints for managing tasks.

All task endpoints require authentication.

**Base URL**

```
/api/tasks
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
| GET | `/api/tasks` | Retrieves a paginated list of tasks. |
| GET | `/api/tasks/{id}` | Retrieves a task by its identifier. |
| PUT | `/api/tasks/{id}` | Updates an existing task. |
| DELETE | `/api/tasks/{id}` | Deletes a task. |

---

## List Tasks

Retrieves a paginated list of tasks.

### Request

**GET** `/api/tasks`

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
GET /api/tasks?pageIndex=1&limit=10&status=Todo&priority=High&sortBy=DueDate&sortDir=Asc
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

**Response Properties**

| Property | Type | Description |
|----------|------|-------------|
| `pageItems` | array | Tasks included in the current page. |
| `totalPages` | integer | Total number of pages. |
| `totalCount` | integer | Total number of tasks. |
| `pageIndex` | integer | Current page number. |
| `hasNext` | boolean | Indicates whether another page exists. |
| `hasPrevious` | boolean | Indicates whether a previous page exists. |

**Possible Errors**

| Status Code | Description |
|-------------|-------------|
| 400 Bad Request | Invalid query parameters. |

See [Error Responses](errors.md) for the response format.

---

## Get Task by ID

Retrieves a single task by its identifier.

### Request

**GET** `/api/tasks/{id}`

**Path Parameters**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | integer | Yes | Unique identifier of the task. |

**Example**

```http
GET /api/tasks/1
```

### Responses

**200 OK**

```json
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
```

**Possible Errors**

| Status Code | Description |
|-------------|-------------|
| 404 Not Found | Task does not exist. |

See [Error Responses](errors.md) for the response format.

---

## Update Task

Updates an existing task.

### Request

**PUT** `/api/tasks/{id}`

**Path Parameters**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | integer | Yes | Unique identifier of the task. |

**Request Body**

```json
{
  "title": "Implement JWT authentication",
  "description": "Add JWT authentication and authorization.",
  "status": "InProgress",
  "priority": "High",
  "dueDate": "2026-08-05"
}
```

**Request Properties**

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `title` | string | Yes | Task title. |
| `description` | string/null | No | Task description. |
| `status` | string | Yes | Current task status. |
| `priority` | string | Yes | Current task priority. |
| `dueDate` | date/null | No | Task due date. |

### Responses

**200 OK**

Task was updated successfully.

```json
{
  "id": 1,
  "projectId": 1,
  "title": "Implement JWT authentication",
  "description": "Add JWT authentication and authorization.",
  "status": "InProgress",
  "priority": "High",
  "dueDate": "2026-08-05",
  "createdAt": "2026-07-24T12:00:00Z",
  "updatedAt": "2026-07-25T10:00:00Z"
}
```

**Possible Errors**

| Status Code | Description |
|-------------|-------------|
| 400 Bad Request | Invalid task data. |
| 404 Not Found | Task does not exist. |

See [Error Responses](errors.md) for the response format.

---

## Delete Task

Deletes an existing task.

### Request

**DELETE** `/api/tasks/{id}`

**Path Parameters**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `id` | integer | Yes | Unique identifier of the task. |

### Responses

**204 No Content**

Task was deleted successfully. No response body is returned.

**Possible Errors**

| Status Code | Description |
|-------------|-------------|
| 404 Not Found | Task does not exist. |

See [Error Responses](errors.md) for the response format.
