# ARCHITECTURE REPORT: LMS MICROSERVICES SYSTEM (LAB 3)

## 1. Service Decomposition (Phân rã Dịch vụ)
Hệ thống quản lý học tập (LMS) monolithic ban đầu từ Bài Lab 2 đã được tái cấu trúc triệt để thành kiến trúc Microservices phân tán bao gồm 4 dịch vụ độc lập:

*   **API Gateway (PRN232.LMS.APIGateway):** 
    *   **Vai trò:** Điểm truy cập duy nhất (Single Entry Point) cho toàn bộ client.
    *   **Nhiệm vụ:** Định tuyến các HTTP request đến đúng microservice mục tiêu và thực hiện kiểm tra, xác thực chữ ký của JWT Access Token trước khi chuyển tiếp yêu cầu vào các mạng dịch vụ nội bộ nhằm bảo vệ hệ thống.
*   **Identity Service (PRN232.LMS.Identity.API):**
    *   **Vai trò:** Quản lý tài khoản và phiên làm việc.
    *   **Nhiệm vụ:** Đăng ký người dùng, xác thực tài khoản (Login) cấp phát mã JWT Access Token và Refresh Token, quản lý quyền hạn (Permissions).
*   **Student Service (PRN232.LMS.Student.API):**
    *   **Vai trò:** Quản lý thông tin học tập của sinh viên.
    *   **Nhiệm vụ:** Quản lý hồ sơ sinh viên, học kỳ (Semesters), môn học (Subjects) và cung cấp gRPC Server để trả lời các truy vấn kiểm tra thông tin sinh viên từ các service khác.
*   **Course Service (PRN232.LMS.Course.API):**
    *   **Vai trò:** Quản lý đào tạo và đăng ký học tập.
    *   **Nhiệm vụ:** Quản lý các lớp học/khóa học (Courses), quản lý đăng ký lớp học (Enrollments), quản lý điểm số (Grades). Dịch vụ này tích hợp gRPC Client để giao tiếp kiểm tra tính hợp lệ của sinh viên từ Student Service.

---

## 2. Database Design (Thiết kế Cơ sở dữ liệu)
Hệ thống tuân thủ nghiêm ngặt mô hình thiết kế **Database-per-service** (Mỗi dịch vụ một cơ sở dữ liệu riêng biệt) chạy trên cùng một máy chủ PostgreSQL để đảm bảo tính cô lập dữ liệu (Loose Coupling):

```
+-----------------------------------------------------------------+
|                         PostgreSQL Server                       |
|                                                                 |
|  +-------------------+  +------------------+  +--------------+  |
|  |  lms_identity_db  |  |  lms_student_db  |  |lms_course_db |  |
|  |                   |  |                  |  |              |  |
|  |  - User           |  |  - Students      |  | - Courses    |  |
|  |  - RefreshToken   |  |  - Semesters     |  | - Enrollments|  |
|  |  - Permission     |  |  - Subjects      |  | - Grades     |  |
|  +-------------------+  +------------------+  +--------------+  |
+-----------------------------------------------------------------+
```

### Các ràng buộc liên kết giữa các cơ sở dữ liệu:
*   Loại bỏ hoàn toàn các ràng buộc vật lý (Foreign Keys) và thuộc tính điều hướng (EF Core Navigation Properties) xuyên biên giới database.
*   Sử dụng **ID logic** (ví dụ: `StudentId` trong bảng `Enrollment` thuộc `lms_course_db` chỉ lưu trữ giá trị kiểu số nguyên để tham chiếu đến ID bên bảng `Students` của `lms_student_db`).
*   Các truy vấn cần đi kèm dữ liệu (như hiển thị thông tin sinh viên khi xem lượt đăng ký học) sẽ được ghép dữ liệu động ở tầng dịch vụ bằng cách gọi gRPC thay vì sử dụng câu lệnh SQL `JOIN`.

---

## 3. API Gateway Configuration (Cấu hình API Gateway)
API Gateway được xây dựng bằng thư viện **YARP (Yet Another Reverse Proxy)** của Microsoft. Hệ thống định tuyến được cấu hình động trong `appsettings.json` của Gateway:

```json
"ReverseProxy": {
  "Routes": {
    "auth-route": {
      "ClusterId": "identity-cluster",
      "Match": { "Path": "/api/auth/{**catch-all}" }
    },
    "student-route": {
      "ClusterId": "student-cluster",
      "AuthorizationPolicy": "JwtPolicy",
      "Match": { "Path": "/api/students/{**catch-all}" }
    },
    "course-route": {
      "ClusterId": "course-cluster",
      "AuthorizationPolicy": "JwtPolicy",
      "Match": { "Path": "/api/courses/{**catch-all}" }
    }
  }
}
```

### Cơ chế xác thực JWT tại Gateway:
*   Gateway được tích hợp Middleware `AddJwtBearer` để giải mã và kiểm tra chữ ký số của JWT token bằng mã khóa bí mật chung (`Jwt:Secret`).
*   Cấu hình chính sách phân quyền `"JwtPolicy"` (yêu cầu người dùng phải xác thực thành công) được áp dụng trực tiếp cho `student-route` và `course-route`.
*   Nếu request không có token hợp lệ, Gateway lập tức phản hồi mã lỗi `401 Unauthorized` về cho client mà không chuyển tiếp request đến microservice đích, giúp giảm tải hệ thống nội bộ.

---

## 4. gRPC Communication Flow (Luồng giao tiếp gRPC)
Khi có yêu cầu đăng ký học mới (Create Enrollment) gửi tới `Course Service`, quy trình giao tiếp đồng thì gRPC (Synchronous gRPC communication) diễn ra như sau:

```
[ Client ]
    |
    |  1. POST /api/courses/enrollments (Body: StudentId: 1, CourseId: 1)
    v
[ API Gateway ]
    |
    |  2. Xác thực JWT & Chuyển tiếp Request
    v
[ Course Service ]
    |
    |  3. Gửi gRPC StudentRequest { StudentId = 1 } (Cổng 8081)
    v
[ Student Service (gRPC Server) ]  --- 4. Truy vấn DB lms_student_db ---> [ Student Table ]
    |                                                                         |
    |  5. Trả về StudentResponse { Exists = true, Fullname, Email }           |
    v                                                                         v
[ Course Service (gRPC Client) ]
    |
    |  6. Kiểm tra Exists = true -> Lưu thông tin đăng ký vào lms_course_db
    v
[ Client ] <--- 7. Trả về HTTP 201 Created (Đăng ký thành công)
```

### Cấu hình cổng gRPC (Kestrel HttpProtocols):
Vì giao thức HTTP/1.1 (Web API) và HTTP/2 (gRPC) không thể chạy chung trên một cổng không có bảo mật mã hóa TLS/ALPN, `Student Service` đã được cấu hình chạy trên hai cổng độc lập:
*   **Port 8080:** Sử dụng giao thức **HTTP/1** để xử lý các truy vấn Web API thường.
*   **Port 8081:** Sử dụng giao thức **HTTP/2** để phục vụ riêng cho các cuộc gọi gRPC hiệu năng cao từ Course Service.
