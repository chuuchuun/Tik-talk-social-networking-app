import { Component, signal } from '@angular/core';
import { DndDirective } from '../../../common-ui/directives/dnd.directive';

@Component({
  selector: 'app-avatar-upload',
  imports: [DndDirective],
  templateUrl: './avatar-upload.component.html',
  styleUrl: './avatar-upload.component.scss'
})
export class AvatarUploadComponent {
preview = signal<string>('/assets/images/profile-placeholder.png')
avatar: File | null= null  

fileBrowserHandler($event: Event) {
  const file = (event?.target as HTMLInputElement)?.files?.[0] as File
  this.proccessFile(file)
  
}

onFileDropped(file: File | null){
  console.log("File dropped", file);
  this.proccessFile(file)
}

proccessFile(file: File | null){
  if(!file || !file.type.match('image')) return

  const reader = new FileReader()

  reader.onload = event =>{
    this.preview.set(event.target?.result?.toString() ?? '')
  }

  reader.readAsDataURL(file)
  this.avatar = file

  console.log("File proccessed", file);
  
}

}
