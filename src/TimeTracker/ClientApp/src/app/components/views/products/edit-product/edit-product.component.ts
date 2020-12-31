import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UiService } from 'src/app/services/ui.service';
import { ProductDto, ProductsClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-edit-product',
  templateUrl: './edit-product.component.html',
  styleUrls: ['./edit-product.component.css']
})
export class EditProductComponent implements OnInit {
  productId: number = 0;
  clientId: number = 0;
  editForm: FormGroup;
  product?: ProductDto;

  constructor(
    private route: ActivatedRoute,
    private uiService: UiService,
    private productsClient: ProductsClient,
    private router: Router
  ) {
    this.editForm = new FormGroup({
      'clientId': new FormControl(null, [Validators.required]),
      'productName': new FormControl(null, [Validators.required])
    });
  }

  ngOnInit(): void {
    this.uiService.showLoader(true);
    this.productId = this.route.snapshot.params.productId;
    
    this.productsClient.getProductById(this.productId).toPromise().then(
      (product: ProductDto) => {
        this.clientId = product?.clientId ?? 0;
        this.product = product;

        this.editForm.patchValue({
          'clientId': product?.clientId ?? 0,
          'productName': product?.productName ?? ''
        });

        this.uiService.hideLoader();
      },
      this.uiService.handleClientError
    );
  }

  onSubmit = () => {
    let updatedProduct = new ProductDto({
      ...this.product,
      ...this.editForm.value
    });

    this.uiService.showLoader(true);
    this.productsClient.updateProduct(updatedProduct).toPromise().then(
      (product: ProductDto) => {
        this.router.navigate(['/products', this.clientId]);
        this.uiService.notify(`Product '${product.productName}' updated`, 1500);
      },
      this.uiService.handleClientError
    );
  }

}
