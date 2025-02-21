import { Component, Input, OnChanges, SimpleChanges, inject } from '@angular/core';
import { Message } from '../../../data/Interfaces/message.interface';
import { Profile } from '../../../data/Interfaces/profile.interface';
import { ProfileService } from '../../../data/services/profile.service';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-message-card',
  templateUrl: './message-card.component.html',
  styleUrls: ['./message-card.component.scss'],
  providers: [DatePipe],
  imports: [RouterLink]
})
export class MessageCardComponent implements OnChanges {
  @Input() message!: Message;
  isSentFromMe: boolean = false;
  profileService = inject(ProfileService);
  avatarUrl: string | null = null;
  username: string | null = null;
  firstName: string | null = null;
  lastName: string | null = null;
  senderId: number | null = null;
  me = this.profileService.me();
  constructor(private datePipe: DatePipe) {}  // Inject DatePipe

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
