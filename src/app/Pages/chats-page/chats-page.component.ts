import { Component, inject, signal } from '@angular/core';
import { ProfileService } from '../../data/services/profile.service';
import { Chat } from '../../data/Interfaces/chat.interface';
import { toSignal } from '@angular/core/rxjs-interop';
import { MessageSpaceComponent } from "./message-space/message-space.component";
import { ChatCardComponentComponent } from "../../common-ui/chat-card-component/chat-card-component.component";
import { AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-chats-page',
  templateUrl: './chats-page.component.html',
  styleUrl: './chats-page.component.scss',
  imports: [MessageSpaceComponent, ChatCardComponentComponent, AsyncPipe],
})
export class ChatsPageComponent {
  selectedChat: Chat | null = null;
  profileService = inject(ProfileService);
  me = this.profileService.me;
  chats = toSignal(this.profileService.chats$); // Auto-updates on change

  ngOnInit() {
    this.profileService.getMyChats().subscribe();
  }

  refreshChatsParent() {
    console.log("Got notified");
    this.profileService.getMyChats().subscribe();
  }

  selectChat(chat: Chat) {
    this.selectedChat = chat;
  }
}
