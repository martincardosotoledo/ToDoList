EJECUCIÓN:
-ejecutar el proyecto ToDoList.WebAPI y probar la aplicación usando la UI generada por swagger

ENDPOINTS + HTTP VERB:
- /Tareas + GET -> lista todas las tareas
- /Tareas + POST -> crea una tarea
- /Tareas/{idTarea} + PUT -> modifica una tarea existente

NOTAS:
-se podrían evitar algunas dependencias, como la de la Web API de ToDoList.DataAccess, usando
contenedores de servicios y IoC.
-debería agregarse un manejador global de errores para que no le llegue al cliente información interna del sistema