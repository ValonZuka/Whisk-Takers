<template>
  <component :is="asChild ? 'slot' : 'button'" :class="buttonClass" v-bind="props" ref="buttonRef" />
</template>

<script setup>
import { ref, defineProps, computed } from 'vue';
import { cva } from 'class-variance-authority';
import { cn } from '@/lib/utils';

const props = defineProps({
  className: String,
  variant: {
    type: String,
    default: 'default',
  },
  size: {
    type: String,
    default: 'default',
  },
  asChild: {
    type: Boolean,
    default: false,
  },
  // Other button attributes can be added here if necessary
});

const buttonVariants = cva(
  "inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg]:size-4 [&_svg]:shrink-0",
  {
    variants: {
      variant: {
        default: "bg-primary text-primary-foreground hover:bg-primary/90",
        destructive: "bg-destructive text-destructive-foreground hover:bg-destructive/90",
        outline: "border border-input bg-background hover:bg-accent hover:text-accent-foreground",
        secondary: "bg-secondary text-secondary-foreground hover:bg-secondary/80",
        ghost: "hover:bg-accent hover:text-accent-foreground",
        link: "text-primary underline-offset-4 hover:underline",
        royal: "bg-gradient-royal text-warm-white shadow-royal hover:shadow-gold transition-all duration-300 transform hover:scale-105",
        gold: "bg-gradient-gold text-royal-green shadow-gold hover:shadow-royal transition-all duration-300",
        elegant: "bg-warm-white text-royal-green border-2 border-royal-green hover:bg-royal-green hover:text-warm-white transition-all duration-300",
      },
      size: {
        default: "h-10 px-4 py-2",
        sm: "h-9 rounded-md px-3",
        lg: "h-11 rounded-md px-8",
        icon: "h-10 w-10",
      },
    },
    defaultVariants: {
      variant: "default",
      size: "default",
    },
  },
);

const buttonClass = computed(() => cn(buttonVariants({ variant: props.variant, size: props.size, className: props.className })));
const buttonRef = ref(null);
</script>