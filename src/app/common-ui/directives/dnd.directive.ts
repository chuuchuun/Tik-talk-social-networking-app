import { Directive, EventEmitter, HostBinding, HostListener, Output } from '@angular/core';

@Directive({
  selector: '[dnd]'
})
export class DndDirective {
@Output()fileDropped = new EventEmitter()


@HostBinding('class.fileover')
fileOver = false

  @HostListener('dragover', ['$event'])
  OnDragOver(event: DragEvent){
    event.stopPropagation();
    event.preventDefault();   
    console.log(event);
    
    this.fileOver = true
  }
  @HostListener('dragleave', ['$event'])
  OnDragLeave(event: DragEvent){
    event.stopPropagation();
    event.preventDefault();   
    
    this.fileOver = false
  }
  @HostListener('drop', ['$event'])
  OnDrop(event: DragEvent){
    event.stopPropagation();
    event.preventDefault(); 
    
    this.fileDropped.emit(event.dataTransfer?.files?.[0])
    this.fileOver = false
  }

}
