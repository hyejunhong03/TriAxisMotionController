# TriAxisMotionController

TCP/IP 소켓 통신 기반의 3축(X/Y/Z) 모터 제어 패널 — C# Windows Forms (.NET Framework)

<br>

## 개요

산업용 모터 드라이버와 TCP 소켓으로 통신하여 X, Y, Z 3개 축을 독립적으로 제어하는 데스크톱 애플리케이션입니다.
스마트팩토리 및 로봇 자동화 환경에서의 사용을 목적으로 개발되었습니다.

<br>

## 주요 기능

- 3축 독립 TCP 소켓 연결 / 해제
- 실시간 텔레메트리 폴링 (위치, 속도, 가속도 등 200ms 주기)
- 절대 위치 이동 (ABS), 상대 위치 이동 (REL)
- 속도(SP), 가속도(AC), 오버라이드(OVRD), S-Curve, 경사(SLP) 파라미터 설정
- REF(원점 복귀), GP0, SM, PWC, PQ 명령 지원
- 직접 명령 입력 기능

<br>

## 기술 스택

| 항목 | 내용 |
|------|------|
| 언어 | C# |
| 프레임워크 | .NET Framework 4.7.2 |
| UI | Windows Forms |
| 통신 | TCP/IP Socket (`System.Net.Sockets`) |
| 비동기 처리 | `async / await` |

<br>

## 프로젝트 구조

```
project_sharp/
├── AxisController.cs     # TCP 연결·명령 전송·텔레메트리 폴링 담당
├── Form1.cs              # UI 이벤트 처리
├── Form1.Designer.cs     # 자동 생성 UI 레이아웃
└── Program.cs            # 진입점
```

**`AxisController`** 클래스가 네트워크 로직을 담당하고, **`Form1`** 은 UI만 담당하도록 분리하여 단일 책임 원칙(SRP)을 적용했습니다.

<br>

## 통신 사양

- 프로토콜: TCP/IP
- 기본 포트: `10001`
- 기본 IP 대역: `192.168.2.100` / `192.168.2.101` / `192.168.2.102`
- 명령 형식: ASCII 문자열 + `\n`

<br>

## 실행 환경

- OS: Windows
- .NET Framework 4.7.2 이상
