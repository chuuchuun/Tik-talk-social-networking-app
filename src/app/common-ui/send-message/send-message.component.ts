import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { Profile } from '../../data/Interfaces/profile.interface';
import { ProfileService } from '../../data/services/profile.service';
import { Chat } from '../../data/Interfaces/chat.interface';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-send-message',
  imports: [ReactiveFormsModule],
  templateUrl: './send-message.component.html',
  styleUrl: './send-message.component.scss'
})
export class SendMessageComponent {
  profileService = inject(ProfileService)
  me = this.profileService.me
  @Output() messageSent = new EventEmitter<void>();
  @Input() chat! : Chat | null; 
  fb = inject(FormBuilder)
  form = this.fb.group({
    text: ['']
  })
  SendMessageToChat() {
    this.form.markAllAsTouched();
    this.form.updateValueAndValidity();
  
    if (this.form.invalid) {
      console.log("Invalid data");
      return;
    }
  
    this.profileService.sendMessage(this.form.value.text!, this.chat!.id).subscribe(() => {
      this.messageSent.emit(); // Notify parent to refresh messages
      console.log("Message sent & refresh triggered");
    });
  }
  
  
}
