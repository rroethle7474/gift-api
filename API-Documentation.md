# Christmas Gift API Documentation

## Authentication
All endpoints require Azure AD B2C authentication. Include the Bearer token in the Authorization header:
```
Authorization: Bearer {your-access-token}
```

## Base URL
Development: `https://localhost:7xxx/api`
Production: `{your-azure-url}/api`

## Endpoints

### Users

#### GET /users
Get all users.
```typescript
interface UserDto {
    userId: number;
    objectId: string;
    username: string;
    email: string;
    isAdmin: boolean;
    isParent: boolean;
    spendingLimit?: number;
    sillyDescription?: string;
}
```

#### GET /users/{id}
Get user by ID.

#### POST /users
Create a new user.
```typescript
interface CreateUserDto {
    username: string;
    password: string;
    email: string;
    isAdmin: boolean;
    isParent: boolean;
    spendingLimit?: number;
    sillyDescription?: string;
}
```

#### PUT /users/{id}
Update user details.
```typescript
interface UpdateUserDto {
    username?: string;
    email?: string;
    isAdmin?: boolean;
    isParent?: boolean;
    spendingLimit?: number;
    sillyDescription?: string;
}
```

#### DELETE /users/{id}
Delete a user.

### Wish List Items

#### GET /wishlist
Get all wish list items.
```typescript
interface WishListItemDto {
    itemId: number;
    userId: number;
    itemName: string;
    description?: string;
    quantity: number;
    productUrl?: string;
    dateAdded: string;
    lastModified: string;
}
```

#### GET /wishlist/{id}
Get wish list item by ID.

#### GET /wishlist/user/{userId}
Get all wish list items for a specific user.

#### POST /wishlist
Create a new wish list item.
```typescript
interface CreateWishListItemDto {
    itemName: string;
    description?: string;
    quantity: number;
    productUrl?: string;
}
```

#### PUT /wishlist/{id}
Update a wish list item.
```typescript
interface UpdateWishListItemDto {
    itemName?: string;
    description?: string;
    quantity?: number;
    productUrl?: string;
}
```

#### DELETE /wishlist/{id}
Delete a wish list item.

### Wish List Submissions

#### GET /wishlistsubmission
Get all wish list submissions.
```typescript
interface WishListSubmissionDto {
    submissionId: number;
    userId: number;
    username: string;
    submissionDate: string;
    parentApprovalStatus?: string;
    adminApprovalStatus?: string;
    lastModified: string;
}
```

#### GET /wishlistsubmission/{id}
Get submission by ID.

#### GET /wishlistsubmission/user/{userId}
Get all submissions for a specific user.

#### GET /wishlistsubmission/pending/parent
Get all submissions pending parent approval.

#### GET /wishlistsubmission/pending/admin
Get all submissions pending admin approval.

#### POST /wishlistsubmission
Create a new submission.
```typescript
interface CreateWishListSubmissionDto {
    userId: number;
}
```

#### PUT /wishlistsubmission/{id}
Update submission status.
```typescript
interface UpdateWishListSubmissionDto {
    parentApprovalStatus?: string;
    adminApprovalStatus?: string;
}
```

#### DELETE /wishlistsubmission/{id}
Delete a submission.

### Hero Content

#### GET /herocontent
Get all hero content items.
```typescript
interface HeroContentDto {
    contentId: number;
    title: string;
    description?: string;
    animationData?: string;
    isActive: boolean;
    createdDate: string;
    lastModifiedDate: string;
}
```

#### GET /herocontent/{id}
Get hero content by ID.

#### GET /herocontent/active
Get currently active hero content.

#### POST /herocontent
Create new hero content.
```typescript
interface CreateHeroContentDto {
    title: string;
    description?: string;
    animationData?: string;
    isActive: boolean;
}
```

#### PUT /herocontent/{id}
Update hero content.
```typescript
interface UpdateHeroContentDto {
    title?: string;
    description?: string;
    animationData?: string;
    isActive?: boolean;
}
```

#### DELETE /herocontent/{id}
Delete hero content.

## Error Handling
All endpoints return standard HTTP status codes:
- 200: Success
- 201: Created
- 400: Bad Request
- 401: Unauthorized
- 403: Forbidden
- 404: Not Found
- 500: Internal Server Error

Error responses include a message:
```typescript
interface ErrorResponse {
    message: string;
}
```

## CORS
CORS is enabled for `http://localhost:4200` in development.

## Authentication Flow
1. User authenticates through Azure AD B2C
2. Frontend receives access token
3. Include token in all API requests:
```typescript
const headers = new HttpHeaders({
    'Authorization': `Bearer ${accessToken}`,
    'Content-Type': 'application/json'
});
```

## Role-Based Access
- Regular users can manage their own wish lists
- Parents can approve/reject wish lists
- Admins have full access to all endpoints

## Date Format
All dates are in ISO 8601 format: `YYYY-MM-DDTHH:mm:ss.sssZ` 