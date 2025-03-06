import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, inject } from '@angular/core';
import { Message } from '../../../data/Interfaces/message.interface';
import { Profile } from '../../../data/Interfaces/profile.interface';
import { ProfileService } from '../../../data/services/profile.service';
import { DatePipe, NgStyle } from '@angular/common';
import { RouterLink } from '@angular/router';
import { delay } from 'rxjs';

@Component({
  selector: 'app-message-card',
  templateUrl: './message-card.component.html',
  styleUrls: ['./message-card.component.scss'],
  providers: [DatePipe],
  imports: [RouterLink, NgStyle]
})
export class MessageCardComponent implements OnChanges {
  @Input() message!: Message;
  @Input() chatId!: number;
  @Output() hasModified = new EventEmitter<void>();
  isSentFromMe: boolean = false;
  profileService = inject(ProfileService);
  avatarUrl: string | null = null;
  username: string | null = null;
  firstName: string | null = null;
  lastName: string | null = null;
  senderId: number | null = null;
  me = this.profileService.me();
  contextMenuVisible = false;
  contextMenuX = 0;
  contextMenuY = 0;

  openContextMenu(event: MouseEvent, message: Message) {
    console.log("Clicked")
    event.preventDefault(); // Prevent default right-click menu
    this.contextMenuVisible = true;
    this.contextMenuX = event.clientX;
    this.contextMenuY = event.clientY;
  }
  deleteMessage(messageId: number) {
    console.log("Delete message:", messageId);
    this.contextMenuVisible = false;
  
    this.profileService.deleteMessage(this.chatId, messageId)
      .pipe(delay(500)) // Delay refresh by 500ms to avoid premature UI update
      .subscribe({
        next: () => {
          console.log("Message deleted successfully");
          this.hasModified.emit();
        },
        error: err => {
          console.error("Error deleting message:", err);
        }
      });
  }
  

  editMessage(message: Message) {
    console.log("Edit message:", message);
    this.contextMenuVisible = false;
    // Implement edit functionality (e.g., open a modal)
  }

  pinMessage(messageId: number) {
    console.log("Pin message:", messageId);
    this.contextMenuVisible = false;
    // Implement pin functionality
  }
  constructor(private datePipe: DatePipe) {
      document.addEventListener('click', () => {
        this.contextMenuVisible = false;
      });
  }  // Inject DatePipe

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['message'] && this.message) {
      this.profileService.getAccount(this.message.userFromId).subscribe(
        res => {
          this.senderId = res.id;
          this.avatarUrl = res.avatarUrl;
          this.username = res.username;
          this.firstName = res.firstName;
          this.lastName = res.lastName;
        }
      );
    }

    if (this.message.userFromId === this.me?.id) {
      this.SentFromMe();
    }
  }

  SentFromMe() {
    this.isSentFromMe = true;
  }

  formatDate(date: Date): string {
    const today = new Date();
    const formattedDate =  this.datePipe.transform(date, "dd.MM.yyyy")
    const formattedTime = this.datePipe.transform(date, 'HH:mm');
    const formatedToday = this.datePipe.transform(today, "dd.MM.yyyy")
    if (formattedDate?.includes(formatedToday!)) {
      return formattedTime || '';
    }
    if (formattedDate?.substring(6,10) === formatedToday?.substring(6,10)){
      return `${this.datePipe.transform(date, 'dd.MM')} ${formattedTime}`    } 
    else {
      return `${this.datePipe.transform(date, 'dd.MM.yyyy')} ${formattedTime}`;
    }
  }
}
