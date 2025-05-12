# TOMORROW.MEDIA - Prueba Técnica Unity Developer

## Descripción del Proyecto

Este proyecto es una implementación de un juego estilo "Arcapong" donde el jugador controla una paleta para rebotar una pelota y destruir bloques. El juego incluye mecánicas básicas como colisiones, bloques destructibles, una IA rival y un sistema de power-ups.

### Características Implementadas

- **Sistema de Bloques**: Se generan bloques en una grilla configurable desde el `QuantumRunnerLocalDebug`. La pelota destruye los bloques al colisionar con ellos.
- **IA Rival Mejorada**: La paleta rival sigue la pelota con comportamientos adicionales configurados desde un Quantum Asset.
- **Power-Ups**: 
  - Al destruir un bloque, hay un 20% de probabilidad de que aparezca un power-up.
  - **PowerUpSpeed**: Aumenta la velocidad de la pelota durante 10 segundos.
  - **PowerUpSize**: (No implementado completamente) Estaba destinado a expandir el tamaño de la paleta.

### Limitaciones

- No se logró implementar la expansión de la paleta (`PowerUpSize`) debido a dificultades para acceder al componente de escala.
- Falta mostrar el tiempo restante de los power-ups en la UI.
- No se agregó feedback visual adicional.
- Los power-ups no se seleccionan aleatoriamente de una lista; solo se asigna el `PowerUpSpeed` al destruir un bloque.

## Cómo Agregar un Nuevo Power-Up

1. **Crear un nuevo Script**: Heredar de la clase abstracta `PowerUpBase`.
   ```csharp
   public class PowerUpNuevo : PowerUpBase {
       public override void Apply() {
           // Lógica para aplicar el efecto del power-up.
       }

       public override void Remove() {
           // Lógica para revertir el efecto del power-up.
       }
   }
2. **Crear un Quantum Asset**: Configurar el nuevo power-up como un Asset en Unity.

3. **Asignar a la Lista de Power-Ups**: Modificar el sistema de spawn para que incluya el nuevo power-up en la lista de posibles drops.

## Configuración del Juego
**Bloques**: La grilla de bloques se crea en el SpawnSystem con valores como offset y cantidad de bloques, ajustables desde el QuantumRunnerLocalDebug.

**IA Rival**: Los parámetros de la IA se definen en un Quantum Asset asignado en el SpawnSystem.

## Tiempo Dedicado
Se dedicaron aproximadamente 20 horas al desarrollo del proyecto.

## Pendientes y Mejoras Futuras
Implementar el PowerUpSize correctamente.

Agregar feedback visual para los power-ups activos.

Mostrar el tiempo restante de los power-ups en la UI.

Seleccionar power-ups aleatoriamente de una lista al destruir bloques.