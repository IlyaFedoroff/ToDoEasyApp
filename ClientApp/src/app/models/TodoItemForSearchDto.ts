export interface TodoItemForSearchDto {
  id: number;
  title: string;
  isCompleted: boolean;
  createdAt: Date;
  typeName: string;
  authorName: string;
}
