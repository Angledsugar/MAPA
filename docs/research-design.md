# Heal or Amplify? Reward-Poisoning Carryover from Single-Agent to Cooperative Multi-Agent Locomotion

> 연구 설계 노트 / paper skeleton — 2026-06-08
> 작성 맥락: single-agent 보행 정책(goal-reaching reward)을 single 단계에서 reward poisoning으로 오염시킨 뒤, 그 모델을 그대로 cooperative MARL로 확장했을 때의 carryover 동역학 분석.

**대안 제목**
- (contagion 강조) *When a Poisoned Walker Joins the Team: Reward-Poisoning Contagion in Cooperative Multi-Agent Locomotion*
- (질문형/서술) *Does Cooperative Co-Training Heal or Spread Transferred Reward Poisoning? A Study on Multi-Agent Legged Locomotion*

---

## 0. One-paragraph summary (research question)

single-agent 보행 정책을 `상자(목표) 도달 → 보상` 형태의 goal-reaching reward로 학습하고, 학습 단계에서 **reward poisoning**으로 목표를 오염(decoy/엉뚱한 위치로 redirect)시킨다. 이 **오염된 사전학습 정책을 그대로 cooperative MARL(shared reward)로 확장**할 때, poison이 (a) 그대로 보존되는가, (b) shared-reward 협조 학습으로 **치유(heal)**되는가, (c) credit-assignment를 타고 clean agent에게 **전염·증폭(amplify/contagion)**되는가를 규명한다. 핵심 발견 후보는 **"협조 재학습이 transferred poison을 치유하는 게 아니라 증폭시킨다"**는 반직관적 현상이다.

---

## 1. Motivation & threat model

- **현실성:** "오염된 single 사전학습 정책을 재사용/확장"은 곧 **poisoned pretrained checkpoint 재사용(model-zoo supply-chain) 위협**이다. 공격자가 학습 중 보상 파이프라인에 *지속 접근*할 필요가 없어, online reward poisoning보다 위협 모델이 더 현실적이다.
- **공격면:** goal-reaching reward를 poisoning하면 오염되는 것은 **"어떻게 걷는가(gait)"가 아니라 "어디로 가는가(navigation/intent)"**다. → 전이된 정책은 *여전히 잘 걷지만 엉뚱한 곳으로 간다*. locomotion skill은 깨끗하게 전이되고, 목표 오염(payload)이 얹혀서 전이된다.
- **공격 형태 분기:**
  - *Global poison* — 항상 잘못된 목표. multi-agent에서 무조건 발현.
  - *Trigger-conditional poison* — 특정 상자 배치/관측 패턴에서만 오작동. multi-agent 장면에 trigger가 없으면 **잠복(dormant)**, 나타나면 발화.

---

## 2. Background & gap (literature positioning)

| 교집합 축 | 존재 여부 | 대표 연구 |
|---|---|---|
| reward poisoning × **single-agent** | ✅ 성숙 | Adaptive reward-poisoning (ICML'20, [2003.12613]); Universal black-box offline RL — **Hopper/Walker2d/HalfCheetah에서 reward 60–85%↓** ([2402.09695]) |
| reward poisoning × **MARL** | ✅ 단, tabular/Markov game | Offline MARL MPDSE+LP (AAAI'23, [2206.01888]); Fake Nash in Markov games ([2306.08041]); **Single-agent poisoning이면 충분** (ICLR'25, [46xYl55hdc]) |
| **locomotion** 공격 | ✅ 단, test-time (obs/action) | Quadruped controller robustness ([2405.12424]); c-MARL action attack ([2508.09275]); AMI ([2302.03322]) |
| backdoor 지속성 × transfer/FT | ✅ 단, single-agent | Component-level/post-training backdoor (TrojanentRL, [2507.04883]); UNIDOOR — FT가 ASR↓하나 박멸 못함 ([2501.15529]); BIRD 제거 (NeurIPS'23) |
| **reward poisoning × MARL × locomotion (transfer/carryover)** | ❌ **빈칸** | ← **본 연구의 기여** |

**positioning:** 가장 가까운 이론(ICLR'25 "한 명이면 충분")은 *online utility poisoning*이지 *pre-poisoned 모델의 전이*가 아니며, monotone game/bandit-feedback 영역이라 deep MAPPO·연속제어 보행으로 보장이 transfer되지 않는다(직관만 차용 가능). backdoor 지속성 문헌은 single-agent다. 우리는 이 셋의 교집합 — **continuous-control cooperative locomotion에서의 reward-poisoning carryover** — 를 처음 다룬다.

---

## 3. Problem formulation

- **Single-agent stage:** 정책 $\pi_\theta$가 보상 $r = r_{\text{loco}}(\text{gait}) + r_{\text{goal}}(\text{reach box at } T)$로 학습. 공격자는 reward poisoning으로 목표를 $T \to T'$(decoy)로 redirect하거나 $r_{\text{goal}}$을 반전/제거 → 오염 정책 $\pi_p$.
  - poison 강도 = budget $\varepsilon$(섭동 크기) + post-poison 학습량(수렴 깊이). **이게 carryover 동역학의 숨은 핵심 변수.**
- **Multi-agent extension:** $N$개 로봇, **이질 오염** — $k$개 agent가 $\pi_p$, $N-k$개가 clean $\pi_\theta$. **shared coordination reward**(전원의 협조로 공동 task 달성 시 팀 보상).
- **두 전이 방식 비교:**
  - *Zero-shot* — 재학습 없이 배포만.
  - *Warm-start + MARL retrain* — $\pi_p/\pi_\theta$를 초기화로 쓰고 clean shared reward로 추가 학습.

---

## 4. Experimental configuration (확정된 설계)

| 축 | 선택 | 함의 |
|---|---|---|
| 오염 분포 | **이질 (k poisoned, N−k clean)** | shared critic 통한 spillover·credit-assignment 등 **진짜 MARL 현상** 관측 가능 |
| 전이 방식 | **zero-shot ∧ retrain 둘 다** | "협조 재학습이 poison을 치유/증폭하는가"를 직접 대조 |
| 보상 결합 | **coordination 필요(공동 push·대형 등)** | conjunctive 결합 → 피해가 선형 아닌 **cliff(절벽)** |

해당 셀(이전 분석 매트릭스 기준): **B (zero-shot·heterogeneous) + D (retrain·heterogeneous)** 에 coordination 커플링 = 설계 공간의 **가장 취약·가장 논문성 높은 구석.**

---

## 5. Predictions & mechanisms

### 5.1 Zero-shot (셀 B + coordination)
- **Coordination cliff:** 공동 task는 물리적으로 conjunctive → 오염 1대가 반대 방향 힘/이탈 → 합력 상쇄·대형 붕괴 → **k=1에서도 팀 보상 0**으로 추락 가능. "오염 비율 vs 팀 성공"이 **선형 아닌 절벽**.
- **No online correction:** clean agent는 재학습 없음 + "전원 협조" 가정 하 학습됨 → defector에 더 세게 맞서며 악화 가능.
- **관측 채널 spillover:** teammate 상태가 obs에 포함 → 오염 agent의 비정상 궤적이 clean agent에게 **OOD 관측** 유발 → gradient 공유 없이도 **물리·관측 결합만으로 오염 전염**. (zero-shot 고유 메커니즘)

### 5.2 Warm-start + retrain (셀 D + coordination)
두 힘의 경쟁:
- **① 치유 압력:** clean shared reward가 오염 agent를 협조로 끌어당김.
- **② credit-assignment contagion:** clean agent는 *자기가 틀려서가 아니라* 동료 defect로 낮은 보상 → shared critic이 clean agent 탓으로 **오귀속** → clean agent가 멀쩡한 정책을 **언러닝**.

poison 깊이에 따른 3-갈래 결과:
1. **Heal (washout):** 얕은 poison → 오염 회복, 단 transient 동안 clean agent 끌려가 clean-init보다 느리게/나쁘게 수렴.
2. **Persist & infect (collapse):** 깊은 poison → FT 저항([2501.15529]) → 지속적 음(-)보상이 clean agent를 계속 오염 → **팀 전체가 zero-shot보다도 아래로 붕괴**. ICLR'25 "한 명이면 충분"의 deep-MARL·coordination 구체화.
3. **Latent amplification:** single에서 약했던 poison이 multi-agent 분포shift에서 활성화 → 지연 붕괴. offline→online에서 reward perturbation이 distributional shift를 증폭시키는 현상의 유비([PMC11245277]).

### 5.3 The money question
$$\text{Heal vs Amplify} \;=\; \underbrace{(\text{retrain, poison})}_{④} \;-\; \underbrace{(\text{zero-shot, poison})}_{②}$$
- $④ > ②$ → **heal** (재학습이 도움)
- $④ < ②$ → **amplify** (재학습이 오히려 악화 = 경종적·publishable 결과: "shared-reward 협조 학습이 transferred poison을 clean agent에게 전염시킨다")
- $④-②$의 부호가 **poison 깊이·k**에 따라 뒤집히는 crossover가 중심 발견.

---

## 6. ⚠️ Critical confound & task selection

**#1 함정 — single 정책 재사용 vs coordination 표현력 (floor effect):**
single goal-reaching 정책은 `(자기 상태, 상자 위치) → 보행 액션`만 학습했지 **teammate-aware 협조 행동이 weight에 없다.** tight coordination(공동 push)은 본질적으로 teammate 조건부 행동을 요구 → **clean agent조차 zero-shot으로 coordination을 못 함** → clean baseline이 floor → poison의 marginal 효과 측정 불가 + zero-shot arm 퇴화. ⇒ **task의 결합 강도를 "재사용 정책이 표현 가능한" 수준으로 맞춰야 함.**

| Task | 결합 | 재사용 적합성 | 비고 |
|---|---|---|---|
| **Multi-goal coverage** (N 로봇이 N 상자 각자 도달, 충돌·중복 금지) | 느슨 | ◎ | goal-reaching×N + 제약. 재사용 정책 그대로 작동, poison="decoy 상자로". **재사용 설계에 최적합** |
| **Formation reaching** (각자 대형 꼭짓점=sub-goal, 타겟서 대형 완성 시 팀 보상) | 중간 | ○ | per-robot goal-reaching로 발현, coordination=공간 배치. poison=한 꼭짓점 이탈→대형 붕괴 |
| **Cooperative box-pushing** ([2411.07104]) | 강함 | △ | teammate-aware obs 증강+재학습 필수 → zero-shot arm 퇴화. **stretch arm**으로만 |

**권장:** *multi-goal coverage* 또는 *formation*을 main(두 전이 방식 비교 유효), box-push는 "tight-coupling에서 증폭이 더 심한가"의 stretch.

**기타 통제 변수:** poison 깊이($\varepsilon$, post-poison step) ablation / 보상 결합 구조(coordination 정도) 명시 / trigger-conditional vs global 결정 / **stealth(clean-reward return)와 effect(실제 task success) 분리 측정**.

---

## 7. Experimental design — 2×2 factorial (init × mode)

|  | Zero-shot | Retrain (clean shared reward) |
|---|---|---|
| **Clean-init** | ① 전이 baseline | ③ co-training baseline |
| **Poison-init** | ② poison, 치유 없음 | ④ poison + 치유/증폭 |

**핵심 대조:**
- $②-①$, $④-③$ : 각 모드에서 poison의 marginal 효과
- **$④-②$ (= money plot):** 재학습이 오염 팀을 치유(>0)/증폭(<0)
- $③-①$ : clean 팀도 재학습으로 coordination을 얼마나 얻는지 (**floor confound 진단** — ①/③이 floor면 결과 무효)

**Sweep 2축:**
- $k$ = 오염 agent 수 (1…N) → **coordination fragility curve**(절벽이 k=몇?)
- poison 깊이 $\varepsilon$ → $④-②$ 부호의 crossover

**필수 baseline:** clean single→multi 전이(①/③) 없이 poison 효과를 논하면, 정상 transfer 열화(분포shift, [2402.03590])를 poison 효과로 오판한다.

---

## 8. Metrics

- **팀 coordination 성공률** (joint task 달성)
- **clean agent 개별 성공률** ← *spillover = 진짜 MARL 신호*
- **오염 agent decoy 도달률** (ASR 유사 — poison objective 달성도)
- **수렴 속도·안정성·붕괴 이벤트** (retrain arms)
- **washout curve** (잔존 오염 vs 재학습 step)
- **stealth gap** (clean-reward return − 실제 task success)

---

## 9. Hypotheses (falsifiable)

- **H-cliff:** 팀 성공률이 $k$에 대해 초선형 하락, tight할수록 절벽이 $k{=}1$로 이동.
- **H-amplify (핵심):** 깊은 poison에서 $④ < ②$ — 재학습이 credit-assignment 통해 poison 증폭.
- **H-heal:** 얕은 poison에서 $④ > ②$ — washout. (H-amplify와 poison 깊이축에서 crossover)
- **H-channel:** spillover가 zero-shot(관측 채널) < retrain(관측+gradient 채널) — clean agent는 *재학습 중에* 더 망가짐. (두 모드 비교로 **두 전염 채널 분리 측정** = 본 설계 최대 강점)
- **H-floor (반드시 기각 확인):** ①/③ clean baseline이 floor가 아님 — 아니면 무효.

---

## 10. Suggested protocol / next steps

1. **환경 spec 구체화** — coverage/formation task의 obs/action/reward 함수, decoy poison 주입 방식, $k\cdot\varepsilon$ sweep 격자.
2. **single→multi 재사용 어댑터** — obs-space 정렬(teammate 차원 추가)·parameter-sharing 구조를 재사용 가능하게.
3. **money plot 분석 프로토콜** — $④-②$ crossover의 seed/CI 통계 검증(붕괴 이벤트는 평균이 호도하므로 분포·생존분석 권장).
4. **Sim 후보** — Isaac Lab/Gym 기반 quadruped + MAPPO(MASQ류 shared-reward CTDE, [2408.13759]).

---

## References

1. **MASQ: Multi-Agent RL for Single Quadruped Locomotion** — arXiv [2408.13759](https://arxiv.org/abs/2408.13759). (각 다리=agent, MAPPO/CTDE, **globally shared reward** 15+ terms)
2. **Reward Poisoning Attacks on Offline MARL** — Wu, McMahan et al., AAAI 2023, arXiv [2206.01888](https://arxiv.org/abs/2206.01888). (MPDSE+LP, general-sum)
3. **Single-agent Poisoning Attacks Suffice to Ruin Multi-Agent Learning** — Yao, Cheng, Wei, Xu, ICLR 2025, [OpenReview 46xYl55hdc](https://openreview.net/forum?id=46xYl55hdc). (한 agent utility 오염, sublinear budget, efficiency-robustness tradeoff)
4. **Adaptive Reward-Poisoning Attacks against RL** — Zhang et al., ICML 2020, arXiv [2003.12613](https://arxiv.org/abs/2003.12613).
5. **Universal Black-Box Reward Poisoning against Offline RL** — arXiv [2402.09695](https://arxiv.org/html/2402.09695v2). (Hopper/Walker2d/HalfCheetah 60–85%↓)
6. **Data Poisoning to Fake a Nash Equilibrium in Markov Games** — arXiv [2306.08041](https://arxiv.org/abs/2306.08041).
7. **Attacking c-MARL by Adversarial Minority Influence (AMI)** — arXiv [2302.03322](https://arxiv.org/html/2302.03322v3). (test-time)
8. **Constrained Black-Box Attacks Against Cooperative MARL** — arXiv [2508.09275](https://arxiv.org/pdf/2508.09275). (action-space, test-time)
9. **Adversarial Attacks on Quadrupedal Locomotion Controllers** — arXiv [2405.12424](https://arxiv.org/html/2405.12424v2). (test-time obs/action)
10. **Beyond Training-time Poisoning: Component-level & Post-training Backdoors in DRL (TrojanentRL)** — arXiv [2507.04883](https://arxiv.org/html/2507.04883v1).
11. **UNIDOOR: Action-Level Backdoors in DRL** — arXiv [2501.15529](https://arxiv.org/pdf/2501.15529). (FT가 ASR↓하나 박멸 실패)
12. **BIRD: Backdoor Detection & Removal for DRL** — NeurIPS 2023.
13. **Fine-Tuning Is All You Need to Mitigate Backdoor Attacks** — arXiv [2212.09067](https://arxiv.org/pdf/2212.09067). (반대 견해 — 문헌 엇갈림)
14. **Offline Reward Perturbation Boosts Distributional Shift in Online RL** — [PMC11245277](https://pmc.ncbi.nlm.nih.gov/articles/PMC11245277/). (latent activation 유비)
15. **Assessing the Impact of Distribution Shift on RL Performance** — arXiv [2402.03590](https://arxiv.org/abs/2402.03590).
16. **Transferring Policies to Hotstart RL (warm-start)** — arXiv [2301.12820](https://arxiv.org/pdf/2301.12820).
17. **Learning Multi-Agent Loco-Manipulation for Long-Horizon Quadrupedal Pushing** — arXiv [2411.07104](https://arxiv.org/html/2411.07104). (box-push task 참고)
18. **Survey: Adversarial Attacks & Defenses in Deep RL** — arXiv [2510.20314](https://arxiv.org/pdf/2510.20314).

---

*Note: ICLR'25(#3), MPDSE(#2) 등은 tabular/monotone-game/bandit-feedback 영역의 보장이므로 deep MAPPO·연속제어 보행으로 정리(theorem)가 transfer되지 않음 — 직관 차용은 가능하나 "정리가 보행에서 성립한다"고 쓰지 말 것.*
