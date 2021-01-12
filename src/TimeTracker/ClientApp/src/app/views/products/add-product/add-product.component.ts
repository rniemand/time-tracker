import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { UiService } from 'src/app/services/ui.service';
import { ProductDto, ProductsClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-add-product',
  templateUrl: './add-product.component.html',
  styleUrls: ['./add-product.component.css']
})
export class AddProductComponent implements OnInit {
  clientId: number = 0;
  addForm: FormGroup;

  constructor(
    private authService: AuthService,
    private uiService: UiService,
    private route: ActivatedRoute,
    private productsClient: ProductsClient,
    private router: Router
  ) {
    this.clientId = this.route.snapshot.params.clientId;

    this.addForm =  new FormGroup({
      'productName': new FormControl(null, [Validators.required]),
      'clientId': new FormControl(this.clientId, [Validators.required])
    });
  }

  ngOnInit(): void {
  }

  onSubmit = () => {
    let productDto = new ProductDto({
      ...this.addForm.value,
      userId: this.authService.currentUser?.id ?? 0
    });
    
    this.uiService.showLoader(true);
    this.productsClient.addProduct(productDto).toPromise().then(
      (success: boolean) => {
        this.router.navigate(['/products', this.clientId]);
        this.uiService.notify(`Product '${productDto.productName}' added`, 1500);
      },
      this.uiService.handleClientError
    );
  }
}
