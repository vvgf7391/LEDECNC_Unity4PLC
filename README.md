# 🏭 Unity + realvirtual.io 工业数字孪生平台

本项目基于 **Unity + realvirtual.io** 构建一个面向工业自动化的数字孪生平台，支持从仿真、虚拟调试到生产运行的全生命周期应用。

---

## 📌 项目架构总览

![Architecture](./ChatGPT%20Image%202026%E5%B9%B43%E6%9C%8817%E6%97%A5%2017_39_15.png)

---

## 🧱 架构分层说明

本系统采用典型的工业数字孪生分层架构：

---

### 1️⃣ User Layer（用户层）

面向不同角色提供系统入口：

- Operator（操作员）
- Engineer（工程师）
- Maintenance（维护人员）
- Management（管理层）

---

### 2️⃣ Application Layer（应用层）

承载具体业务场景：

- **3D HMI**：三维可视化操作界面
- **Virtual Commissioning**：虚拟调试
- **Training Simulator**：培训仿真系统
- **Production Monitoring**：生产监控
- **AI Optimization**：AI优化分析

---

### 3️⃣ Digital Twin Engine（数字孪生引擎）

核心运行层（Unity + realvirtual）：

- Kinematics System（运动学系统）
- Behavior Simulation（行为仿真）
- Sensor Simulation（传感器仿真）
- Motion Control Model（运动控制模型）
- Physics Engine（物理引擎）

📌 核心特点：
> 基于 **Signal Driven Simulation（信号驱动仿真）**

---

### 4️⃣ Industrial Interface（工业接口层）

通过 realvirtual 提供的接口连接工业系统：

- Siemens S7
- Beckhoff ADS
- OPC UA
- MQTT
- REST API

📌 当前阶段：
- ✅ 已支持 OPC UA
- 🚧 后续扩展 ADS / S7

---

### 5️⃣ Industrial Systems（工业系统层）

实际工业数据来源：

- PLC
- Robots
- MES
- SCADA
- IoT Sensors

---

## ⚙️ 当前开发阶段

### ✅ 已完成

- Unity 项目初始化
- realvirtual.io 插件导入
- 架构设计

### 🚧 进行中（第一阶段）

👉 **工业通信层（OPC UA）**

目标：

- 建立 OPC UA 通信
- 构建 Signal Manager
- 实现 Unity ↔ PLC 数据同步

---

## 🧩 核心模块设计

### 🔹 1. OPC UA Client

负责：

- 连接 OPC Server
- 读取 / 写入变量
- 数据同步

---

### 🔹 2. Signal Manager（核心）

统一管理所有工业信号：

```text
PLC → OPC UA → SignalManager → realvirtual.Signal → 设备行为
