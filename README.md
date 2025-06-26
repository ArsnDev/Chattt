# 💬 Chattt (C# WinForms 채팅 클라이언트)

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## 📝 프로젝트 소개

`Chattt`는 C# Windows Forms 기반으로 개발된 **간단한 실시간 채팅 클라이언트 애플리케이션**입니다. 사용자는 회원가입 및 로그인을 통해 채팅방에 접속하며, 다른 사용자들과 실시간으로 메시지를 주고받고 현재 접속 중인 참가자 목록을 확인할 수 있습니다.

본 프로젝트는 클라이언트 UI와 서버 로직(In-Memory DB 기반)을 모두 구현하여, **로컬 네트워크 또는 외부 네트워크를 통해 실제 채팅 기능을 테스트**할 수 있습니다.

![image](https://github.com/user-attachments/assets/ec11f6fc-8b49-4ff0-b45c-59858b6cfd38)
![image](https://github.com/user-attachments/assets/d285c322-28b5-4445-b51e-83bc21da9886)

---

## ✨ 주요 기능

* **사용자 인증:**
    * 회원가입: 새로운 사용자 ID와 비밀번호를 등록 (서버 메모리(In-Memory DB)에 저장).
    * 로그인: 등록된 ID와 비밀번호를 통해 서버에 인증. 중복 로그인 방지.
* **실시간 채팅:**
    * 메시지 입력 및 전송: 사용자가 입력한 메시지를 서버를 통해 다른 참가자들에게 실시간으로 전송.
    * 채팅 내역 표시: 수신된 모든 메시지를 채팅창(`RichTextBox`)에 표시.
* **참가자 관리:**
    * 실시간 참가자 목록: 현재 채팅방에 접속 중인 사용자 목록을 `ListBox`에 표시.
    * 입장/퇴장 알림: 새로운 사용자의 입장 및 기존 사용자의 퇴장 정보를 모든 참가자에게 실시간으로 알림.
* **화면 전환:** 로그인 성공 시 로그인 화면에서 채팅방 화면으로 자동 전환.

---

## 🚀 시작하기

이 프로젝트를 로컬 또는 외부 환경에서 실행하려면 Visual Studio와 .NET 개발 환경이 필요합니다.

### 📋 전제 조건

* Visual Studio 2019 이상
* .NET (또는 .NET Framework, 프로젝트 설정에 따름) SDK

### 📦 빌드 및 실행

1.  **리포지토리 클론:**
    ```bash
    git clone [프로젝트_레포지토리_URL]
    cd Chattt
    ```
2.  **Visual Studio에서 솔루션 열기:**
    * `Chattt.sln` 파일을 Visual Studio로 엽니다.
3.  **서버 프로젝트 설정 및 실행:**
    * 솔루션 탐색기에서 `Chattt_Server` 프로젝트를 마우스 오른쪽 버튼으로 클릭 후 `시작 프로젝트로 설정(Set as Startup Project)`합니다.
    * `Ctrl+F5` 또는 `F5`를 눌러 서버를 실행합니다. (콘솔 창이 열림)
    * **주의:** 서버는 클라이언트 접속 전에 항상 실행 중이어야 합니다.
4.  **클라이언트 프로젝트 설정 및 실행:**
    * 솔루션 탐색기에서 `Chattt` 프로젝트를 마우스 오른쪽 버튼으로 클릭 후 `시작 프로젝트로 설정(Set as Startup Project)`합니다.
    * `Ctrl+F5` 또는 `F5`를 눌러 클라이언트를 실행합니다. (WinForms 앱이 열림)

### ⚙️ 네트워크 설정 (외부 접속 시)

친구와 외부 네트워크에서 채팅하려면 서버를 실행하는 컴퓨터의 공인 IP 주소와 **포트 포워딩(`TCP:12345`)** 설정이 필요합니다.

1.  **서버 컴퓨터의 로컬 IP 고정** 및 **공유기(라우터)에서 포트 `12345 (TCP)`를 서버 컴퓨터의 로컬 IP로 포워딩** 설정.
2.  서버 컴퓨터의 **Windows 방화벽**에서 **인바운드 규칙**으로 포트 `12345 (TCP)`를 허용.
3.  클라이언트 프로젝트의 `MainForm.cs` 파일 내 `SERVER_IP` 상수를 **서버 컴퓨터의 공인 IP 주소**로 변경 후 클라이언트 재빌드 및 배포.

---

## 🛠️ 개발 환경

* **언어**: C#
* **프레임워크**: .NET (Windows Forms)
* **개발 도구**: Visual Studio

---

## Chattt 클라이언트-서버 패킷 설계

콜론(`:`)으로 구분된 텍스트 메시지를 사용합니다.

---

### 1. 클라이언트 (C) → 서버 (S) 요청

| 명령어 (`COMMAND`) | 필드 1 (`DATA1`) | 필드 2 (`DATA2`) | 예시 |
| :----------------- | :--------------- | :--------------- | :------------------- |
| `LOGIN`            | 사용자 ID        | 비밀번호         | `LOGIN:user1:pass1` |
| `REGISTER`         | 사용자 ID        | 비밀번호         | `REGISTER:newuser:newpass` |
| `CHAT_MESSAGE`     | 발신자 ID        | 메시지 내용      | `CHAT_MESSAGE:user1:Hello!` |
| `REQUEST_PARTICIPANTS` | (없음)           | (없음)           | `REQUEST_PARTICIPANTS` |

---

### 2. 서버 (S) → 클라이언트 (C) 응답 및 알림

| 명령어 (`COMMAND`) | 필드 1 (`DATA1`) | 필드 2 (`DATA2`) | 예시 |
| :----------------- | :--------------- | :--------------- | :------------------- |
| `LOGIN_SUCCESS`    | 사용자 ID        | (없음)           | `LOGIN_SUCCESS:user1` |
| `LOGIN_FAIL`       | 실패 이유        | (없음)           | `LOGIN_FAIL:InvalidUser` (ID/PW 불일치)<br>`LOGIN_FAIL:AlreadyLoggedIn` (중복 로그인) |
| `REGISTER_SUCCESS` | 사용자 ID        | (없음)           | `REGISTER_SUCCESS:newuser` |
| `REGISTER_FAIL`    | 실패 이유        | (없음)           | `REGISTER_FAIL:IDExists` (ID 중복) |
| `CHAT_MESSAGE`     | 발신자 ID        | 메시지 내용      | `CHAT_MESSAGE:user2:Hi there!` |
| `USER_JOINED`      | 사용자 ID        | (없음)           | `USER_JOINED:user3` |
| `USER_LEFT`        | 사용자 ID        | (없음)           | `USER_LEFT:user3` |
| `PARTICIPANTS_LIST`| 콤마로 구분된 ID 목록 | (없음)           | `PARTICIPANTS_LIST:user1,user2,user3` |
| `ERROR`            | 오류 메시지      | (없음)           | `ERROR:InvalidCommand` <br> `ERROR:NotLoggedIn` |

## 📄 라이선스

이 프로젝트는 MIT 라이선스에 따라 배포됩니다. 자세한 내용은 `LICENSE` 파일을 참조하세요.
