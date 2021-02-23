import { Component, EventEmitter, forwardRef, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { MatFormFieldAppearance } from '@angular/material/form-field';
import { IntListItem, ProductDto, ProductsClient } from 'src/app/time-tracker-api';

@Component({
  selector: 'app-product-selector',
  templateUrl: './product-selector.component.html',
  styleUrls: ['./product-selector.component.css'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ProductSelectorComponent),
      multi: true
    }
  ]
})
export class ProductSelectorComponent implements OnInit, ControlValueAccessor, OnChanges {
  @Output('changed') onChange = new EventEmitter<number>();
  @Input('clientId') clientId: number = 0;
  @Input('appearance') appearance: MatFormFieldAppearance = "outline";
  @Input('class') class: string = "";
  
  productId: number = 0;
  productName: string = '';
  loading: boolean = true;
  entries: IntListItem[] = [];
  label: string = 'Select a client first';
  private lookup: { [key: number]: string } = {};

  private _onChangeFn = (_: any) => { };

  constructor(
    private productsClient: ProductsClient
  ) { }
  

  ngOnInit(): void {
    if(isNaN(this.clientId))
      this.clientId = 0;
  }

  selectionChange = () => {
    this.productName = '';
    if(this.lookup.hasOwnProperty(this.productId)) {
      this.productName = this.lookup[this.productId];
    }

    if (this._onChangeFn) {
      this._onChangeFn(this.productId);
    }

    this.onChange.next(this.productId);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(changes.hasOwnProperty('clientId')) {
      this.refreshProducts();
    }
  }

  writeValue(obj: any): void {
    if(typeof(obj) === 'object' && isNaN(obj)) {
      this.productId = 0;
    }
    else if(typeof(obj) === 'string') {
      this.productId = parseInt(obj);
    }
    else {
      this.productId = obj;
    }
  }

  registerOnChange(fn: any): void {
    this._onChangeFn = fn;
  }

  registerOnTouched(fn: any): void { }


  // Internal methods
  private refreshProducts = () => {
    this.loading = true;
    this.entries = [];
    this.lookup = {};

    if(isNaN(this.clientId) || this.clientId <= 0) {
      return;
    }

    this.label = 'Select a client first';
    this.productsClient.listClientProducts(this.clientId).toPromise().then(
      (products: IntListItem[]) => {
        products.forEach((p: IntListItem) => {
          this.lookup[p?.value ?? 0] = p?.name ?? '';
        });

        this.entries = products;

        if(this.productId <= 0 && this.entries.length > 0) {
          this.setProductId(this.entries[0].value ?? 0);
        }

        this.label = 'Select a product';
        this.handleHardCodedProductId();
        this.loading = false;
      },
      (error: any) => {
        this.loading = false;
      }
    );
  }

  private setProductId = (productId: number) => {
    this.productId = productId;
    
    this.productName = '';
    if(this.lookup.hasOwnProperty(productId)) {
      this.productName = this.lookup[productId];
    }
    
    this.selectionChange();
  }

  private hasProductId = (productId: number) => {
    if(this.entries.length == 0)
      return false;

    for(var i = 0; i < this.entries.length; i++) {
      if(this.entries[i].value === productId) {
        return true;
      }
    }

    return false;
  }

  private handleHardCodedProductId = () => {
    if(this.productId <= 0 || this.entries.length == 0)
      return;

    if(!this.hasProductId(this.productId))
      return;

    this.setProductId(this.productId);
  }
}
