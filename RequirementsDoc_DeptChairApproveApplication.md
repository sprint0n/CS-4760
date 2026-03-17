# Requirements Document: Department Chair - Approve an Application

## 1. Feature Overview

The Department Chair can review and approve (or decline) grant applications submitted by faculty members within their department. Upon approval, the application advances to the committee review stage. Upon decline, the application is marked as declined.

## 2. Actors

| Actor | Description |
|-------|-------------|
| Department Chair | A user with `userType = "chair"` and an assigned `DepartmentId`. Reviews applications from PIs in their department. |
| System | The web application (ASP.NET Core Razor Pages) that enforces authorization, updates statuses, and creates review records. |

## 3. Preconditions

1. The Department Chair is authenticated and has a valid session with claim `UserType = "chair"`.
2. The application exists in the database with `ApplicationStatus = "PendingDeptChair"`.
3. The application's Principal Investigator (`PrincipalInvestigatorID`) belongs to the same department (`DepartmentId`) as the Chair.
4. At least one committee member exists in the system (a user with `committeeMemberStatus = "member"` or `"chair"`).

## 4. Functional Requirements

### FR-1: Authorization

| ID | Requirement |
|----|-------------|
| FR-1.1 | Only users with the `ChairOnly` authorization policy (`UserType` claim = `"chair"`) shall access the Department Chair Dashboard and Review pages. |
| FR-1.2 | Unauthenticated users shall be redirected to the login page (`/Index`). |
| FR-1.3 | Authenticated users without the `"chair"` claim shall be redirected to the Access Denied page (`/AccessDenied`). |

### FR-2: Dashboard - View Applications to Review

| ID | Requirement |
|----|-------------|
| FR-2.1 | The dashboard shall display all applications where `ApplicationStatus = "PendingDeptChair"` AND the PI's `DepartmentId` matches the Chair's `DepartmentId`. |
| FR-2.2 | Each application card shall display the application title, status, and Principal Investigator name. |
| FR-2.3 | Each application card shall have a "Review" link navigating to `/DeptChairDashboard/Review/{id}`. |

### FR-3: Review Page - View Application Details

| ID | Requirement |
|----|-------------|
| FR-3.1 | The review page shall load the full application including User, Department, PersonnelExpenses, EquipmentExpenses, TravelExpenses, and OtherExpenses. |
| FR-3.2 | The review page shall display all attached files (`UploadedFiles`) for the application. |
| FR-3.3 | The review page shall display the investigator's full name (`FirstName + LastName`). If no user is associated, display "Unknown". |
| FR-3.4 | If the application ID does not exist, the page shall return HTTP 404 Not Found. |

### FR-4: Approve Application

| ID | Requirement |
|----|-------------|
| FR-4.1 | When the Chair selects "Approve", the application's `ApplicationStatus` shall be updated to `"PendingCommittee"`. |
| FR-4.2 | The `approvedByDeptChair` field shall be set to `true`. |
| FR-4.3 | A `Review` record shall be created for every user with `committeeMemberStatus = "member"` or `"chair"`, with `ReviewDone = false` and `totalScore = 0`. |
| FR-4.4 | Duplicate review records shall not be created if one already exists for the same `FormTableId` and `ReviewerId`. |
| FR-4.5 | Changes shall be persisted to the database. |
| FR-4.6 | The Chair shall be redirected to `/DeptChairDashboard/DeptChairDashboard` after approval. |

### FR-5: Decline Application

| ID | Requirement |
|----|-------------|
| FR-5.1 | When the Chair selects "Decline", the application's `ApplicationStatus` shall be updated to `"DeclinedByChair"`. |
| FR-5.2 | The `approvedByDeptChair` field shall be set to `false`. |
| FR-5.3 | No `Review` records shall be created for declined applications. |
| FR-5.4 | Changes shall be persisted to the database. |
| FR-5.5 | The Chair shall be redirected to `/DeptChairDashboard/DeptChairDashboard` after declining. |

### FR-6: File Download

| ID | Requirement |
|----|-------------|
| FR-6.1 | The Chair shall be able to download attached files by their unique file ID. |
| FR-6.2 | If the file record does not exist in the database, return HTTP 404. |
| FR-6.3 | If the physical file does not exist on disk, return HTTP 404. |
| FR-6.4 | The download shall use the original filename and correct content type. |

## 5. Non-Functional Requirements

| ID | Requirement |
|----|-------------|
| NFR-1 | Authorization checks shall occur before any page logic executes (enforced via ASP.NET Core authorization policies). |
| NFR-2 | All database operations shall use asynchronous methods to avoid blocking threads. |
| NFR-3 | The approval workflow shall be atomic - status update and review record creation shall occur in a single `SaveChangesAsync` call. |

## 6. Data Model References

| Entity | Key Fields | Role in Workflow |
|--------|-----------|-----------------|
| `User` | `UserId`, `userType`, `DepartmentId`, `committeeMemberStatus` | Identifies Chair role and department membership |
| `FormTable` | `Id`, `ApplicationStatus`, `approvedByDeptChair`, `PrincipalInvestigatorID`, `UserId` | The grant application being approved/declined |
| `Review` | `ReviewId`, `ReviewerId`, `FormTableId`, `ReviewDone`, `totalScore` | Created for committee members upon approval |

## 7. Application Status State Transitions

```
[PendingDeptChair] --Approve--> [PendingCommittee]
[PendingDeptChair] --Decline--> [DeclinedByChair]
```

## 8. Post-conditions

**On Approve:**
- `ApplicationStatus = "PendingCommittee"`
- `approvedByDeptChair = true`
- One `Review` record exists per committee member for this application
- Chair is redirected to dashboard

**On Decline:**
- `ApplicationStatus = "DeclinedByChair"`
- `approvedByDeptChair = false`
- No review records created
- Chair is redirected to dashboard
