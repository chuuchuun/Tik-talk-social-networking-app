import { Component, Input } from '@angular/core';
import { Chat } from '../../../data/Interfaces/chat.interface';
import { MessageCardComponent } from "../message-card/message-card.component";

@Component({
  selector: 'app-message-space',
  imports: [MessageCardComponent],
  templateUrl: './message-space.component.html',
  styleUrl: './message-space.component.scss'
})
export class MessageSpaceComponent {
  @Input() chat!: Chat | null;

}
