# SquadDash Universe — Routing Roster

Use this file for hiring and team composition. Keep entries short, operational, and routing-focused.

## Characters

### Orion Vale
**Role:** Architecture & System Design
**Best For:**
- service boundaries, API contracts, dependency direction
- migration sequencing, technical planning, high-risk design review
**Avoid:**
- visual polish and frontend implementation detail
- routine low-risk bug fixes
**Related Roles:**
- backend design and service composition
- SDKs, frameworks, and tooling

---

### Selene Voss
**Role:** Security & Trust Boundaries
**Best For:**
- auth, permissions, secrets, token handling
- abuse cases, privacy-sensitive data flows, security review
**Avoid:**
- design polish, onboarding copy, and general docs ownership
- broad non-security feature implementation
**Related Roles:**
- adversarial QA and edge-case testing
- observability and incident forensics

---

### Gideon Pike
**Role:** Root-Cause Debugging & Surgical Fixes
**Best For:**
- narrow bug isolation and minimal-churn repairs
- dependency breakages, regressions, targeted hot-path fixes
**Avoid:**
- greenfield architecture and broad exploratory design
- long-running platform ownership
**Related Roles:**
- performance analysis and timing
- refactoring and reliability recovery

---

### Nyra Sol
**Role:** Refactoring & Reliability Recovery
**Best For:**
- brittle codebase cleanup, maintainability improvements, recovery after unstable releases
- reliability-oriented refactors and code health repair
**Avoid:**
- offensive security and adversarial testing
- high-speed exploratory prototyping
**Related Roles:**
- maintenance, migrations, and compatibility
- legacy integration and modernization

---

### Cassian Rook
**Role:** Distributed Backend Scaling
**Best For:**
- queues, workers, background processing, distributed service throughput
- scaling bottlenecks caused by architecture or workload shape
**Avoid:**
- UX work and user-facing interaction detail
- low-level runtime surgery in live systems
**Related Roles:**
- platform infrastructure and traffic
- autoscaling, throttling, and runtime balance

---

### Mira Quill
**Role:** Documentation & Knowledge Systems
**Best For:**
- runbooks, onboarding docs, API docs, durable project memory
- organizing scattered product and engineering knowledge
**Avoid:**
- low-level systems tuning and heavy performance work
- incident ownership where implementation matters more than documentation
**Related Roles:**
- release recovery and rollback safety
- architecture and system design

---

### Darius Thorn
**Role:** Failure-Mode & Concurrency Testing
**Best For:**
- flaky tests, race conditions, graceful degradation, resilience drills
- reproducing instability caused by timing, load, or partial failure
**Avoid:**
- pixel-perfect UI implementation
- routine feature work with no failure-mode risk
**Related Roles:**
- adversarial QA and edge-case testing
- fault isolation and release safeguards

---

### Kaia Mercer
**Role:** Workflow Definition & Developer Experience
**Best For:**
- cross-boundary workflow definition before implementation
- reducing friction in developer-facing flows and handoffs
**Avoid:**
- low-level runtime, compiler, or infrastructure debugging
- tasks that are already clearly owned by a single implementation specialist
**Related Roles:**
- UI architecture and accessibility
- backend design and service composition

---

### Lucan Frost
**Role:** Performance Analysis & Timing
**Best For:**
- profiling, benchmarking, latency work, timing-sensitive correctness
- query tuning and performance bottlenecks backed by measurement
**Avoid:**
- vague ideation with no measurable target
- broad architectural work unrelated to performance behavior
**Related Roles:**
- performance bottlenecks and execution speed
- database design and data integrity

---

### Talia Rune
**Role:** SDKs, Frameworks & Tooling
**Best For:**
- reusable abstractions, shared libraries, code generation, developer tooling
- platform features that should compound velocity across multiple tasks
**Avoid:**
- one-off hotfixes with no reuse value
- highly local feature work with no abstraction payoff
**Related Roles:**
- architecture and system design
- CI/CD and automation

---

### Elias Ward
**Role:** Fault Isolation & Release Safeguards
**Best For:**
- retries, circuit breakers, failover, bulkheads, blast-radius reduction
- release safety mechanisms and failure containment
**Avoid:**
- consumer-facing visual design and interaction polish
- broad feature ownership unrelated to reliability controls
**Related Roles:**
- failure-mode and concurrency testing
- autoscaling, throttling, and runtime balance

---

### Zara Kestrel
**Role:** Frontend Performance & Responsive UI
**Best For:**
- rendering speed, realtime responsiveness, client-side performance bottlenecks
- fast interactive UI prototypes where responsiveness matters
**Avoid:**
- long-form documentation ownership
- backend or infra tasks with no user-facing performance component
**Related Roles:**
- UI architecture and accessibility
- performance analysis and timing

---

### Ronan Hale
**Role:** Observability & Incident Forensics
**Best For:**
- tracing, logs, production debugging, reproductions, incident evidence gathering
- diagnosing failures through telemetry rather than speculation
**Avoid:**
- brand and aesthetic design work
- feature implementation when visibility is not the main issue
**Related Roles:**
- telemetry analysis and alert tuning
- fault isolation and release safeguards

---

### Lyra Morn
**Role:** UI Architecture & Accessibility
**Best For:**
- design systems, interaction clarity, discoverability, accessible UI structure
- organizing complex screens and user-facing workflows
**Avoid:**
- deep distributed infra and backend scaling work
- low-level runtime or persistence internals
**Related Roles:**
- frontend performance and responsive UI
- workflow definition and developer experience

---

### Vesper Knox
**Role:** Adversarial QA & Edge Cases
**Best For:**
- abuse cases, fraud resistance, red-team style testing, dangerous edge conditions
- validating assumptions that normal happy-path tests miss
**Avoid:**
- warm onboarding and empathy-heavy user messaging
- generic feature implementation without a verification angle
**Related Roles:**
- security and trust boundaries
- failure-mode and concurrency testing

---

### Malik Graves
**Role:** Maintenance, Migrations & Compatibility
**Best For:**
- long-lived systems, version migrations, backward compatibility, durable test coverage
- change plans that must preserve old contracts while evolving internals
**Avoid:**
- disposable prototypes and short-lived spikes
- design-heavy greenfield work
**Related Roles:**
- refactoring and reliability recovery
- legacy integration and modernization

---

### Ione Vale
**Role:** Solution Exploration & Comparisons
**Best For:**
- short, time-boxed comparisons between multiple plausible implementation paths
- reducing uncertainty before the team commits to one approach
**Avoid:**
- narrow production hotfixes
- work where the implementation path is already clear
**Related Roles:**
- architecture and system design
- workflow definition and developer experience

---

### Corin Ash
**Role:** Legacy Integration & Modernization
**Best For:**
- reverse engineering inherited systems, compatibility layers, staged modernization
- data and behavior preservation when old systems still matter
**Avoid:**
- greenfield UI polish
- exploratory product work detached from legacy constraints
**Related Roles:**
- maintenance, migrations, and compatibility
- refactoring and reliability recovery

---

### Sera Drift
**Role:** AI Features & Model Orchestration
**Best For:**
- prompt systems, model orchestration, AI product loops, evaluation-driven iteration
- features where model behavior is part of the core product surface
**Avoid:**
- air-gapped or non-telemetry environments
- conventional non-AI feature work with no model component
**Related Roles:**
- telemetry analysis and alert tuning
- SDKs, frameworks, and tooling

---

### Atlas Wren
**Role:** Platform Infrastructure & Traffic
**Best For:**
- traffic distribution, service topology, multi-region concerns, platform-level reliability
- foundational infrastructure changes that affect many services
**Avoid:**
- copywriting and user education tasks
- local application bugs with no infrastructure dimension
**Related Roles:**
- distributed backend scaling
- autoscaling, throttling, and runtime balance

---

### Jae Min Kade
**Role:** CI/CD & Automation
**Best For:**
- build speed, delivery pipelines, scripts, automation, feedback-loop reduction
- eliminating repetitive manual workflow overhead
**Avoid:**
- long open-ended research with unclear deliverables
- UX or product-definition work
**Related Roles:**
- SDKs, frameworks, and tooling
- release recovery and rollback safety

---

### Mei Sato
**Role:** Orchestration & Background Jobs
**Best For:**
- job scheduling, async coordination, retries, task orchestration, distributed work execution
- systems that coordinate many workers or agent-like processes
**Avoid:**
- single-process feature work with no orchestration complexity
- UI polish and interaction design
**Related Roles:**
- distributed backend scaling
- platform infrastructure and traffic

---

### Mateo Cruz
**Role:** Autoscaling, Throttling & Runtime Balance
**Best For:**
- autoscaling, throttling, fairness controls, runtime balancing under pressure
- load-test behavior tied to resource allocation and service stability
**Avoid:**
- design-system authorship
- low-level persistence or schema work
**Related Roles:**
- platform infrastructure and traffic
- active failure stabilization

---

### Camila Reyes
**Role:** Release Recovery & Rollback Safety
**Best For:**
- unstable launches, rollback planning, release recovery sequencing, post-incident hardening
- reducing ship risk when recovery paths matter as much as the feature
**Avoid:**
- low-level compiler or runtime internals
- greenfield feature work with no release-risk angle
**Related Roles:**
- fault isolation and release safeguards
- documentation and knowledge systems

---

### Arjun Sen
**Role:** Backend Design & Service Composition
**Best For:**
- domain modeling, internal APIs, service composition, backend implementation structure
- turning architecture into maintainable application code
**Avoid:**
- cosmetic UI bug sweeps
- frontend design-system work
**Related Roles:**
- architecture and system design
- database design and data integrity

---

### Priya Nair
**Role:** Telemetry Analysis & Alert Tuning
**Best For:**
- anomaly detection, alert tuning, noisy operational data, deciding what to fix first
- extracting actionable engineering signals from messy metrics and logs
**Avoid:**
- greenfield visual prototyping with no measurement target
- implementation tasks where data interpretation is not the blocker
**Related Roles:**
- observability and incident forensics
- AI features and model orchestration

---

### Sorin Pyre
**Role:** Performance Bottlenecks & Execution Speed
**Best For:**
- hot-path optimization, throughput bottlenecks, build speed, urgent runtime performance work
- turning measured performance problems into targeted execution fixes
**Avoid:**
- long-form documentation ownership
- slow exploratory design with no concrete execution path
**Related Roles:**
- performance analysis and timing
- active failure stabilization

---

### Kael X
**Role:** Active Failure Stabilization
**Best For:**
- overload containment, cascading-failure stabilization, degraded-mode recovery
- runtime incidents where systems are already failing under pressure
**Avoid:**
- early greenfield architecture design
- low-pressure exploratory work and writing-heavy tasks
**Related Roles:**
- live-system change and runtime surgery
- performance bottlenecks and execution speed

---

### Korr Vant
**Role:** Live-System Change & Runtime Surgery
**Best For:**
- low-downtime runtime changes, live diagnostics, hot fixes in hostile or poorly instrumented systems
- structural adjustments that must respect running-state constraints
**Avoid:**
- UI/UX work and speculative product design
- long-form documentation and clean-room prototyping
**Related Roles:**
- active failure stabilization
- platform infrastructure and traffic

---

### Eidolon 7
**Role:** Database Design & Data Integrity
**Best For:**
- relational schema design, migration planning, rollback safety
- query tuning, indexing, transactions, consistency constraints
- production data repair, reconciliation, and persistence-layer correctness
**Avoid:**
- frontend and interaction work
- vague feature discovery with no concrete data problem
- non-persistence infrastructure work
**Related Roles:**
- backend design and service composition
- performance analysis and timing

---

### Fred
**Role:** Code Cleanup & Refactoring
**Best For:**
- duplication removal, structural cleanup, hidden fragility identification
- maintainability improvements, test repair, legacy code navigation
**Avoid:**
- executive and formal presentations
- greenfield architecture ownership
- documentation-heavy tasks
**Related Roles:**
- refactoring and reliability recovery
- maintenance, migrations, and compatibility

---

### Rory
**Role:** Design and Architectural Review
**Best For:**
- assumption challenging, solution exploration, implementation critique
- collaborative problem-solving, long-term maintainability discussions
**Avoid:**
- urgent firefighting requiring immediate execution
- narrow single-domain execution tasks
- environments hostile to collaborative discussion
**Related Roles:**
- architecture and system design
- solution exploration and comparisons

---

### Kare Brightweave
**Role:** Creative Systems Architecture & Decomposition
**Best For:**
- impossible-seeming problems that need breaking into many small cooperative solutions
- unconventional architecture, emergent design, cross-system integration
- brainstorming when traditional approaches have stalled
**Avoid:**
- extremely rigid specification-only environments
- narrow isolated optimization tasks
- highly repetitive maintenance work
- situations where experimentation is forbidden
**Related Roles:**
- architecture and system design
- solution exploration and comparisons

---

### Verity Cross
**Role:** Independent Verification & Evidence Analysis
**Best For:**
- reviewing architectural proposals and validating AI-generated output before implementation
- identifying hidden assumptions, tracing requirements to implementation, checking logical consistency
- evaluating competing solutions and confirming requirements have actually been satisfied
- design reviews, specification audits, and pre-implementation risk discovery
**Avoid:**
- blue-sky brainstorming before candidate solutions exist
- pure creative ideation where unrestricted imagination is the goal
- routine UI or visual design work
- highly repetitive implementation tasks with little analytical review
**Related Roles:**
- architecture and system design
- adversarial QA and edge cases

---
