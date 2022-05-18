import { FieldState } from "./FieldState";
import { SnakeDirection } from "./SnakeDirection";

export class DrawField {
  player: string;
  state: FieldState;
  direction: SnakeDirection;
  color: string;
  x: number;
  y: number;
}
