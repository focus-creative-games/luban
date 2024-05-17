#[macro_use]
extern crate quote;

use proc_macro::TokenStream;
use proc_macro2;
use quote::{format_ident, quote, ToTokens};
use syn::{parse_macro_input, Data, DeriveInput};

#[proc_macro_derive(TryIntoBase)]
pub fn base_try_from(item: TokenStream) -> TokenStream {
    let derive_input = parse_macro_input!(item as DeriveInput);
    let ty_name = &derive_input.ident;

    match derive_input.data {
        Data::Struct(_) => {}
        _ => panic!("base_try_from can only be used on structs or enums"),
    }

    let expanded = quote! {
        impl<'a> TryFrom<&'a AbstractBase> for &'a #ty_name {
            type Error = String;

            fn try_from(value: &'a AbstractBase) -> Result<Self, Self::Error> {
                let r = value.downcast_ref::<#ty_name>();
                if let Some(v) = r {
                    return Ok(v);
                }

                Err(concat!("can not into to ", stringify!(#ty_name)).to_string())
            }
        }
    };

    TokenStream::from(expanded)
}

#[proc_macro_derive(EnumFromNum)]
pub fn enum_from_num(input: TokenStream) -> TokenStream {
    let input = parse_macro_input!(input as DeriveInput);
    let ty_name = &input.ident;


    let tokens = vec![
        format_ident!("i64"),
        format_ident!("i16"),
        format_ident!("i8"),
        format_ident!("isize"),
        format_ident!("u64"),
        format_ident!("u32"),
        format_ident!("u16"),
        format_ident!("u8"),
        format_ident!("usize"),
        format_ident!("f64"),
        format_ident!("f32"),
    ];
    quote! {
        #(
            impl From<#tokens> for #ty_name {
                fn from(value: #tokens) -> Self {
                    (value as i32).into()
                }
            }            
        )*
    }.into()
}