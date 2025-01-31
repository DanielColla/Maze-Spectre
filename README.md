# 🌀 Maze Game

## 📝 Descripción  
**Maze Game** es un juego de laberinto en el que los jugadores deben encontrar la salida 🏁 mientras evitan trampas ⚠️ y utilizan poderes especiales ✨ para superar obstáculos. El juego genera laberintos aleatorios y cuenta con un sistema de trampas que afectan el movimiento de los jugadores.  

## 📜 Scripts incluidos  

### 1. **🔍 Logica.cs**  
Este script se encarga de la generación y manipulación del laberinto, así como de la colocación de trampas y la lógica de movimiento del jugador controlado por la IA 🤖.  

#### ⚙️ Funcionalidades principales:  
- **Generación del laberinto** 🏗️: Se utiliza un algoritmo para crear caminos aleatorios.  
- **Colocación de trampas** ⚠️: Se generan trampas en ubicaciones aleatorias dentro del laberinto.  
- **Movimiento del jugador IA** 🤖: Implementa el algoritmo A* para encontrar la salida.  
- **Verificación de solubilidad** ✅: Usa DFS para asegurarse de que el laberinto tenga una solución.  

#### 🕳️ Tipos de trampas:  
- **Trampas normales** ⚠️: Afectan a los jugadores cuando las pisan.  
- **Swap Traps** 🔄: Intercambian la posición del jugador con otra ubicación al activarse.  
- **Knockback Traps** 💥: Empujan al jugador hacia atrás cuando las pisa.  

### 2. **🎮 Player.cs**  
Define la clase `Player`, que representa a los jugadores dentro del laberinto. Cada jugador tiene habilidades especiales con un número limitado de usos.  

#### 🔹 Atributos del jugador:  
- **Nombre** 🏷️  
- **Descripción del poder especial** ✨  
- **Historia del personaje** 📖  

#### 🛠️ Habilidades disponibles:  
- **Teleport** 🌀: Permite al jugador teletransportarse.  
- **Teleport hacia la salida** 🎯: Acerca al jugador a la salida.  
- **Swap Positions** 🔄: Intercambia la posición con otro jugador.  
- **Stun Other Player** ⚡: Aturde a otro jugador por un turno.  
- **Place Random Trap** ⚠️: Coloca una trampa en una ubicación aleatoria.  

Cada una de estas habilidades puede usarse un máximo de tres veces.  

## 🎲 Cómo jugar  
1. Se genera un laberinto aleatorio 🏗️ al iniciar la partida.  
2. Los jugadores deben encontrar la salida 🏁 mientras evitan trampas ⚠️.  
3. La IA 🤖 usará el algoritmo A* para encontrar la salida.  
4. Los jugadores pueden usar sus habilidades especiales estratégicamente para avanzar o afectar a sus oponentes.  

## 💻 Requisitos  
- **C#** (para modificar o mejorar los scripts y ejecutarlo en la consola de cualquier editor de código).
- **dotnet add package Spectre.Console**  agregar este comando para bajar la libreria de Spectre 


## 🚀 Notas adicionales  
Este proyecto está en desarrollo y se planea agregar más tipos de trampas, efectos de habilidades y mejoras en la IA para una mejor experiencia de juego.  



