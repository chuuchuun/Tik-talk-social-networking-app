import { Component, Input, OnChanges, SimpleChanges, inject } from '@angular/core';
import { Message } from '../../../data/Interfaces/message.interface';
import { Profile } from '../../../data/Interfaces/profile.interface';
import { ProfileService } from '../../../data/services/profile.service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-message-card',
  templateUrl: './message-card.component.html',
  styleUrls: ['./message-card.component.scss'],
  providers: [DatePipe] // Add DatePipe as a provider to the component
})
export class MessageCardComponent implements OnChanges {
  @Input() message!: Message;
  isSentFromMe: boolean = false;
  profileService = inject(ProfileService);
  avatarUrl: string | null = null;
  username: string | null = null;
  firstName: string | null = null;
  lastName: string | null = null;
  me = this.profileService.me();
  constructor(private datePipe: DatePipe) {}  // Inject DatePipe

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['message'] && this.message) {
      this.profileService.getAccount(this.message.userFromId).subscribe(
        res => {
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

  // Format date depending on whether it's today or from a previous day
  formatDate(date: Date): string {
    console.log("Date is" ,date.getFullYear, date.getMonth, date.getDate)
    const today = new Date();
    console.log("Today is", today.getFullYear, today.getMonth, today.getDate)
    const formattedDate =  this.datePipe.transform(date, "dd-MM-yyyy")
    const formattedTime = this.datePipe.transform(date, 'HH:mm');
    const formatedToday = this.datePipe.transform(today, "dd-MM-yyyy")
    // Check if the message date is today
    if (formattedDate?.includes(formatedToday!)) {
      // If today, show only time (HH:mm)
      return formattedTime || '';
    } else {
      // If not today, show date and time (dd-MM-yyyy HH:mm)
      return `${this.datePipe.transform(date, 'dd-MM-yyyy')} ${formattedTime}`;
    }
  }
}
