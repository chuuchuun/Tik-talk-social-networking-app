import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-avatar-upload',
  imports: [],
  templateUrl: './avatar-upload.component.html',
  styleUrl: './avatar-upload.component.scss'
})
export class AvatarUploadComponent {
preview = signal<string>('/assets/images/profile-placeholder.png')
  fileBrowserHandler($event: Event) {
  const file = (event?.target as HTMLInputElement)?.files?.[0] as File
  
  if(!file || !file.type.match('image')) return

  const reader = new FileReader()

  reader.onload = event =>{
    this.preview.set(event.target?.result?.toString() ?? '')
  }

  reader.readAsDataURL(file)
}

}
