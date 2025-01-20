export interface TodoItem {
  id?: number;
  title: string;
  isCompleted: boolean;
  typeId: number;
  isEditing?: boolean
}
