# src

구현 모듈 (stack 결정 후 채움 — IsaacLab 쿼드러페드 vs Unity ML-Agents).

계획 모듈:
- `envs/` — coordination task 래퍼 (coverage / formation / boxpush), teammate-aware obs
- `poison/` — reward-poisoning 주입기 (goal→decoy redirect, ε·depth·trigger 통제)
- `transfer/` — single→multi 어댑터 (obs-space 정렬, parameter-sharing, warm-start init)
- `agents/` — MAPPO/CTDE (또는 PPO/SAC) policy·critic
- `metrics/` — team success / clean-agent 개별 성공(spillover) / decoy 도달률(ASR) / washout curve / stealth gap

> 공통 인터페이스를 먼저 고정하면 stack 교체 비용↓.
