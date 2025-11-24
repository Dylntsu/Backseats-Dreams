# 🚗 Backseats Dreams - Endless Runner 2D

> Un juego de carrera infinita desarrollado en Unity con enfoque en optimización y escalabilidad.

![Unity](https://img.shields.io/badge/Unity-2022%2B-black?style=for-the-badge&logo=unity)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp)
![Status](https://img.shields.io/badge/Status-Terminado-success?style=for-the-badge)

## 🎮 Descripción

**Backseats Dreams** es un Endless Runner 2D donde el jugador debe esquivar obstáculos, recolectar monedas y utilizar potenciadores estratégicos para alcanzar la mayor distancia posible. El proyecto destaca por su arquitectura de software optimizada para móviles y sistemas de progresión persistente.

## ✨ Características Principales

* **🏃‍♂️ Mecánicas de Movimiento:** Salto, doble salto, deslizamiento (crouch) y mecánica de caída rápida ("fast fall").
* **⚡ Sistema de Potenciadores:**
    * **Imán:** Atrae monedas cercanas.
    * **Escudo:** Protege de un impacto.
    * **Doble Monedas:** Multiplica el valor de recolección.
* **🛒 Tienda y Progresión:** Sistema de economía con `PlayerPrefs` para guardar monedas y mejorar la duración de los potenciadores (Sistema de Niveles).
* **🚀 Optimización (Object Pooling):** Implementación de un sistema de reciclaje de objetos para obstáculos y monedas, eliminando el `Instantiate/Destroy` constante para mejorar el rendimiento (Garbage Collection).
* **📉 Dificultad Progresiva:** La velocidad del juego aumenta gradualmente con el tiempo.

## 🛠️ Tecnologías y Patrones

* **Engine:** Unity (2D).
* **Lenguaje:** C#.
* **Patrones de Diseño:**
    * **Singleton:** Para `GameManager`, `UIManager` y `ShopManager`.
    * **Object Pooling:** Para la generación procedural de terreno y obstáculos.
    * **Observer/Event Driven (Simulado):** Comunicación desacoplada entre colisiones y UI.
* **Persistencia de Datos:** Uso de `PlayerPrefs` para guardar HighScore, Monedas Totales y Niveles de Mejoras.

## 📂 Estructura del Proyecto

```bash
├── Scripts/
│   ├── Managers/
│   │   ├── GameManager.cs      # Control central del flujo de juego
│   │   ├── SpawnManager.cs     # Lógica de Object Pooling y generación
│   │   ├── ShopManager.cs      # Lógica de compra y mejoras
│   │   └── UIManager.cs        # Control de HUD y feedback visual
│   ├── Player/
│   │   └── PlayerController.cs # Física, inputs y estados del jugador
│   └── Objects/
│       ├── PowerUp.cs          # Lógica modular de potenciadores
│       └── MoveLeft.cs         # Comportamiento de desplazamiento del entorno
