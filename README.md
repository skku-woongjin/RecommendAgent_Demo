# Recommend Agent Demo Project

> [RecommendAgent_Train](https://github.com/skku-woongjin/RecommendAgent_Train)에서 훈련한 장소 추천 모델(Recommend Agent)을 실행시켜볼 수 있는 유니티 프로젝트입니다. <br/>
> 모델의 state로 넣어줄 유저 로그를 직접 생성해보고, 로그에 따른 모델의 추천을 확인할 수 있습니다. 

## 실행 화면
<img width="678" alt="실행화면" src="https://user-images.githubusercontent.com/60357053/209494711-d4910412-fd6b-4a6d-abe4-81e2f038b46c.png">

<br />

## Prerequisite

`Unity` (version: 2021.3.6f1)
<br />

## Getting Started


### Clone Repository

```shell script
$ git clone https://github.com/skku-woongjin/RecommendAgent_Demo.git
$ cd RecommendAgent_Demo
```

### How to Run

Unity Hub에서 프로젝트 경로 추가 후 실행

## 사용법 
- **노란 깃발**: 장소를 표현합니다. 깃발 안에는 장소 id가, 깃발 왼쪽에는 방문 횟수가 표시됩니다. 화면 오른쪽에 장소별 방문 시간도 표시됩니다.
  - 깃발을 클릭하면, 유저가 해당 장소에 방문했다가 랜덤한 위치로 빠져나옵니다.
- **유저 로그**: 유저가 장소에 방문하면, 해당 장소의 특성값에 따라 그래프에 파란 점이 찍힙니다. 
  - 아래의 "추천" 버튼을 누르면 모델이 유저 선호 최고 지점을 예측하여 그래프 상에 빨간 점으로 표시합니다.
  - 유저 선호와 가장 유사한 장소는 파란 체크로, 유저 선호와 가장 먼 장소는 빨간 체크로 깃발 위에 표시됩니다. 
- **기타 버튼**
  - **장소/유저 랜덤배치**: 장소/유저의 위치를 랜덤으로 설정합니다. 
  - **방문시간 랜덤생성**: 모든 장소의 방문시간 특성값을 랜덤으로 설정합니다.
  - **방문시간 초기화**: 모든 장소의 방문시간 특성값을 0으로 설정합니다. 
  - **로그 랜덤생성**: 유저가 랜덤한 장소 20곳을 방문하게 하여 로그를 생성합니다. 
    - **움직임 스킵**: 로그 랜덤 생성 시 유저의 움직임을 보여주지 않고 바로 생성합니다. 
  - **로그 초기화**: 유저 로그와 발자취를 모두 지웁니다. 

## 파일 구조

```
.
├── README.md
├── Assets
│   ├── Demo
│   │   ├── Materials
│   │   ├── Models
│   │   │   ├── Junwon.onnx
│   │   │   └── getmean_tri.onnx
│   │   ├── Prefabs
│   │   │   ├── Dest.prefab
│   │   │   ├── Slider.prefab
│   │   │   ├── TrailPoint.prefab
│   │   │   ├── dot_agent.prefab
│   │   │   ├── dot_answer.prefab
│   │   │   ├── dot_userlog.prefab
│   │   │   └── user.prefab
│   │   ├── Scripts
│   │   │   ├── ClickDetector.cs
│   │   │   ├── Flag.cs
│   │   │   ├── GameManager.cs
│   │   │   ├── JunwonAgent.cs
│   │   │   ├── ModelOverrider.cs
│   │   │   ├── RecommendAgent.cs
│   │   │   ├── TrailEnergyDecrease.cs
│   │   │   ├── TrailGenerator.cs
│   │   │   └── TrailPoint.cs
│   │   └── TrainScene
│   │       └── NavMesh.asset
│   ├── ML-Agents
│   │   └── Timers
│   └── Plugins
│       ├── Borodar
│       │   └── RainbowHierachy
│       └── TextMesh Pro
├── Packages
└── ProjectSettings
```

<br />

- [Assets/Demo](https://github.com/skku-woongjin/RecommendAgent_Demo/tree/main/Assets/Demo) : Demo Scene 을 구성하는 Asset 모음
- [Assets/Demo/Materials](https://github.com/skku-woongjin/RecommendAgent_Demo/tree/main/Assets/Demo/Materials) : 3D 오브젝트에 씌울 Material 모음
- [Assets/Demo/Models](https://github.com/skku-woongjin/RecommendAgent_Demo/tree/main/Assets/Demo/Models) : 훈련된 모델 모음
    - Junwon.onnx: 비교 모델, #visit, duration, distance 가 모두 높은 장소 추천
    - getmean_tri.onnx: [RecommendAgent_Train]()을 통해 훈련된 모델
- [Assets/Demo/Scripts](https://github.com/skku-woongjin/RecommendAgent_Demo/tree/main/Assets/Demo/Scripts) : C# 스크립트 모음

## Components

- **[ClickDetector.cs](https://github.com/skku-woongjin/RecommendAgent_Demo/blob/main/Assets/Demo/Scripts/ClickDetector.cs)**
  - 장소 클릭 시 유저가 해당 장소로 이동하도록 함
  <br />
- **[Flag.cs](https://github.com/skku-woongjin/RecommendAgent_Demo/blob/main/Assets/Demo/Scripts/Flag.cs)**
  - 장소 class 정의
  <br />
- **[GameManager.cs](https://github.com/skku-woongjin/RecommendAgent_Demo/blob/main/Assets/Demo/Scripts/GameManager.cs)** 
  - 장소 특성 저장, 장소 방문 처리
  - UI 업데이트
  - 장소 배치, 초기화 등
  <br />
- **[JunwonAgent.cs](https://github.com/skku-woongjin/RecommendAgent_Demo/blob/main/Assets/Demo/Scripts/JunwonAgent.cs)** 
  - Junwon.onnx의 input과 output 처리
  <br />
- **[RecommendAgent.cs](https://github.com/skku-woongjin/RecommendAgent_Demo/blob/main/Assets/Demo/Scripts/RecommendAgent.cs)** 
  - getmean_tri.onnx의 input과 output 처리 
  <br />
- **[TrailEnergyDecrease.cs](https://github.com/skku-woongjin/RecommendAgent_Demo/blob/main/Assets/Demo/Scripts/TrailEnergyDecrease.cs),[TrailGenerator.cs](https://github.com/skku-woongjin/RecommendAgent_Demo/blob/main/Assets/Demo/Scripts/TrailGenerator.cs),[TrailPoint.cs](https://github.com/skku-woongjin/RecommendAgent_Demo/blob/main/Assets/Demo/Scripts/TrailPoint.cs)** 
  - 유저 이동 시 나타나는 발자취 처리 
  <br />
    
