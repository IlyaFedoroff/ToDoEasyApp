export interface UserWithCompletedTodosDto {
  userId: string,
  firstName: string,
  lastName: string,
  completedTodosCount: number;

  incompletedTodosCount: number;
  lastActivity?: Date;
}
