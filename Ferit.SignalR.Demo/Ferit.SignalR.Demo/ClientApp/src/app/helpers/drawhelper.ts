import { ElementRef, Inject, Injectable } from "@angular/core";
import { DrawField } from "src/models/DrawField";
import { FieldState } from "src/models/FieldState";
import { RoomState } from "../../models/RoomState";
import { SnakeDirection } from "../../models/SnakeDirection";

@Injectable({
  providedIn: "root",
})
export class DrawHelper {
  private ctx: CanvasRenderingContext2D;
  private canvasSize: number;
  private fieldSize: number;
  private hFieldSize: number;
  private qFieldSize: number;

  constructor(@Inject("GAME_FILED_SIZE") private gameFieldSize: number) { }

  initialize(canvas: ElementRef<HTMLCanvasElement>) {
    this.ctx = canvas.nativeElement.getContext("2d");
    this.setSizes(canvas);
  }

  render(v: RoomState) {
    this.clearCanvas();

    v.drawFields.forEach((d) => {
      if (d.state == FieldState.SnakeHead) {
        this.drawSnakeHead(d);
      }
      else if (d.state == FieldState.Food) {
        this.drawFood(d);
      }
      else {
        this.drawBody(d);
      }
    });
  }

  private setSizes(canvas: ElementRef<HTMLCanvasElement>) {
    var size = Math.min(canvas.nativeElement.parentElement.offsetWidth - 10,
      canvas.nativeElement.parentElement.offsetHeight - 10);

    canvas.nativeElement.width = size;
    canvas.nativeElement.height = size;

    this.canvasSize = size;

    this.fieldSize = canvas.nativeElement.width / this.gameFieldSize;
    this.hFieldSize = this.fieldSize / 2;
    this.qFieldSize = this.fieldSize / 4;
  }

  private clearCanvas() {
    this.ctx.clearRect(0, 0, this.canvasSize, this.canvasSize);
    this.ctx.fillStyle = 'rgba(52, 140, 49, 0.09)';
    this.ctx.fillRect(0, 0, this.canvasSize, this.canvasSize);
  }

  private drawFood(d: DrawField) {
    this.ctx.beginPath();
    this.ctx.arc(d.x * this.fieldSize + this.hFieldSize, d.y * this.fieldSize + this.hFieldSize, this.hFieldSize, 0, 2 * Math.PI);
    this.ctx.fillStyle = d.color;
    this.ctx.strokeStyle = "rgba(1, 1, 1, 0)";
    this.ctx.fill();
  }

  private drawBody(d: DrawField) {
    this.ctx.fillStyle = d.color;
    this.ctx.stroke();
    this.ctx.fillRect(d.x * this.fieldSize, d.y * this.fieldSize, this.fieldSize, this.fieldSize);
  }

  private drawSnakeHead(d: DrawField) {
    this.ctx.fillStyle = d.color;

    var x = d.x * this.fieldSize;
    var y = d.y * this.fieldSize;

    this.ctx.fillRect(x, y, this.fieldSize, this.fieldSize);

    //draw eye
    if (d.direction == SnakeDirection.Up) {
      x += this.hFieldSize;
    } else if (d.direction == SnakeDirection.Down) {
      x += this.hFieldSize;
      y += this.fieldSize;
    } else if (d.direction == SnakeDirection.Left) {
      y += this.hFieldSize;
    } else if (d.direction == SnakeDirection.Right) {
      x += this.fieldSize;
      y += this.hFieldSize;
    }

    this.ctx.beginPath();
    this.ctx.arc(x, y, this.qFieldSize, 0, 2 * Math.PI, false);
    this.ctx.fillStyle = "yellow";
    this.ctx.fill();
    this.ctx.strokeStyle = "black";
    this.ctx.stroke();
  }
}
