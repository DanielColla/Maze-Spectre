# 🌀 Maze Game

## 📝 Descripción  
**Maze Game** es un juego de laberinto en el que los jugadores deben encontrar la salida 🏁 mientras evitan trampas ⚠️ y utilizan poderes especiales ✨ para superar obstáculos. El juego genera laberintos aleatorios y cuenta con un sistema de trampas y habilidades especiales para aumentar la estrategia y dificultad.

---

## 📜 Scripts incluidos  

### 1. **🔍 Logica.cs**  
Este script maneja la generación y manipulación del laberinto, la colocación de trampas y la lógica de movimiento del jugador controlado por la IA 🤖.  

#### ⚙️ Funcionalidades principales:  
- **Generación del laberinto** 🏗️: Usa el algoritmo de Prim para crear caminos aleatorios y asegurarse de que el laberinto sea resoluble.  
- **Colocación de trampas** ⚠️: Se generan trampas en ubicaciones aleatorias dentro del laberinto.  
- **Movimiento del jugador IA** 🤖: Implementa el algoritmo A* para encontrar la salida de manera eficiente.  
- **Verificación de solubilidad** ✅: Usa DFS para garantizar que el laberinto tenga una solución.  

#### 🕳️ Tipos de trampas:  
- **Trampas normales** ⚠️: Afectan a los jugadores cuando las pisan.  
- **Swap Traps** 🔄: Intercambian la posición del jugador con otra ubicación al activarse.  
- **Knockback Traps** 💥: Empujan al jugador hacia atrás cuando las pisa.  

---

### 2. **🎮 Player.cs**  
Define la clase `Player`, que representa a los jugadores dentro del laberinto. Cada jugador tiene habilidades especiales con un número limitado de usos.  

#### 🔹 Atributos del jugador:  
- **Nombre** 🏷️  
- **Posición dentro del laberinto** 🗺  
- **Habilidades especiales disponibles** ✨  
- **Cantidad de usos restantes para cada habilidad**

#### 🛠️ Habilidades disponibles:  
- **Teleport** 🛋️: Permite al jugador teletransportarse a una ubicación aleatoria.  
- **Teleport hacia la salida** 🎯: Acerca al jugador a la salida usando el algoritmo A*.  
- **Swap Positions** 🔄: Intercambia la posición con otro jugador.  
- **Stun Other Player** ⚡: Aturde a otro jugador, haciéndolo perder un turno.  
- **Place Random Trap** ⚠️: Coloca una trampa en una ubicación aleatoria del laberinto.  

Cada una de estas habilidades puede usarse un máximo de tres veces.  

---

### 3. **🎮 GameManager.cs**  
Controla la lógica principal del juego, incluyendo la gestión de turnos y la verificación de condiciones de victoria.  

#### ⚙️ Funcionalidades principales:  
- **Gestor de turnos** ⏳: Alterna entre los jugadores, permitiendo que cada uno realice una acción por turno.  
- **Condición de victoria** 🏆: El juego finaliza cuando un jugador alcanza la salida del laberinto.  
- **Balance de la IA** 🤖: La IA recibe un turno extra al final para equilibrar la falta de habilidades.  

---

### 4. **🎮 IA.cs**  
Define el comportamiento de la inteligencia artificial del juego.  

#### 🤖 Características de la IA:  
- **Algoritmo A*** 🔍: La IA encuentra el camino más corto hasta la salida.  
- **Estrategia de movimiento** 🏃: Evita trampas y busca la ruta óptima según su heurística.  
- **Turno extra** ⏳: Al final del juego, la IA se mueve dos veces para balancear la dificultad.  

---

## 🎮 Cómo jugar  
1. Al iniciar la partida, se genera un laberinto aleatorio 🏗️.  
2. Se elige el modo de juego:  
   - **Jugador vs IA** 🤖  
   - **Jugador vs Jugador** 🎭🎭  
3. Los jugadores deben encontrar la salida 🏁 mientras evitan trampas ⚠️.  
4. Pueden moverse con las teclas:
   - **W** ⬆️ - Arriba  
   - **S** ⬇️ - Abajo  
   - **A** ⬅️ - Izquierda  
   - **D** ➡️ - Derecha  
5. La IA 🤖 usa el algoritmo A* para moverse de forma eficiente.  
6. Se pueden utilizar habilidades especiales para influir en la partida.  

---

## 💻 Requisitos  
- **C#** (para modificar o mejorar los scripts y ejecutarlo en la consola de cualquier editor de código).  
- **.NET SDK 6.0 o superior** 🛠️  
- **Spectre.Console** 🔧: Para mejorar la interfaz visual en la consola. Instalar con:  
  ```sh
  dotnet add package Spectre.Console
  ```

---

## 🚀 Notas adicionales  
Este proyecto está en desarrollo y se planea agregar:  
- ✨ Más tipos de trampas y efectos de habilidades.  
- 🤖 Mejoras en la IA para una experiencia de juego más desafiante.  
- 🎮 Nuevos modos de juego y niveles de dificultad.  

---

💪 **¡Prepárate para desafiar tu mente y escapar del laberinto!** 🏁

