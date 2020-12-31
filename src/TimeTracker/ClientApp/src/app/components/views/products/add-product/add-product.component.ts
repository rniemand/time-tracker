import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from 'src/app/services/auth.service';
import { UiService } from 'src/app/services/ui.service';
import { ProductDto } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-add-product',
  templateUrl: './add-product.component.html',
  styleUrls: ['./add-product.component.css']
})
export class AddProductComponent implements OnInit {
  addForm: FormGroup;

  constructor(
    private authService: AuthService,
    private uiService: UiService
  ) {
    this.addForm =  new FormGroup({
      'productName': new FormControl(null, [Validators.required]),
      'clientId': new FormControl(null, [Validators.required])
    });
  }

  ngOnInit(): void {
  }

  onSubmit = () => {
    let productDto = new ProductDto({
      ...this.addForm.value,
      userId: this.authService.currentUser?.id ?? 0
    });
    
    console.log(productDto);
  }

}
