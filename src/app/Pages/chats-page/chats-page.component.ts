import { Component, inject, signal } from '@angular/core';
import { ProfileService } from '../../data/services/profile.service';
import { Chat } from '../../data/Interfaces/chat.interface';
import { MessageSpaceComponent } from "./message-space/message-space.component";
import { ChatCardComponentComponent } from "../../common-ui/chat-card-component/chat-card-component.component";
import { AsyncPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-chats-page',
  templateUrl: './chats-page.component.html',
  styleUrl: './chats-page.component.scss',
  imports: [ReactiveFormsModule, MessageSpaceComponent, ChatCardComponentComponent, AsyncPipe],
})
export class ChatsPageComponent {
  selectedChat: Chat | null = null;
  profileService = inject(ProfileService);
  me = this.profileService.me;
  chats = toSignal(this.profileService.chats$); // Auto-updates on change
  fb = inject(FormBuilder);
  searchForm = this.fb.group({
    username: ['']
  });

  searchFormSub!: Subscription;

  constructor() {
    this.refreshChatsParent();
  }

  ngOnDestroy(): void {
    if (this.searchFormSub) {
      this.searchFormSub.unsubscribe();
    }
  }

  refreshChatsParent() {
    console.log("Got notified");
    this.profileService.getMyChats().subscribe();
  }

  selectChat(chat: Chat) {
    this.selectedChat = chat;
  }

  onSearch() {
    const formValue = this.searchForm.value;
    console.log("Searching for:", formValue.username);
    
    if (this.searchFormSub) {
      this.searchFormSub.unsubscribe(); // Avoid multiple subscriptions
    }

    this.searchFormSub = this.profileService.filterChats(formValue).subscribe();
  }
}
