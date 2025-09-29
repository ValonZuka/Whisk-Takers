<template>
  <!-- Card Component -->
  <div
    :class="cn('rounded-lg border bg-card text-card-foreground shadow-sm', cardProps.class)"
    v-bind="cardProps"
  >
    <slot />
  </div>
</template>

<script setup>
import { cn } from '@/lib/utils'

// Card Props
const cardProps = defineProps({
  class: String,
  // Allow other HTML attributes
  ...Object.fromEntries(
    Object.keys(HTMLElement.prototype).map(key => [key, { type: [String, Number, Boolean, Function], default: undefined }])
  )
})

// CardHeader Component
const cardHeaderProps = defineProps({
  class: String,
  ...Object.fromEntries(
    Object.keys(HTMLElement.prototype).map(key => [key, { type: [String, Number, Boolean, Function], default: undefined }])
  )
})

const CardHeader = {
  name: 'CardHeader',
  props: cardHeaderProps,
  setup(props, { slots }) {
    return () => (
      <div
        class={cn('flex flex-col space-y-1.5 p-6', props.class)}
        {...props}
      >
        {slots.default?.()}
      </div>
    )
  }
}

// CardTitle Component
const cardTitleProps = defineProps({
  class: String,
  ...Object.fromEntries(
    Object.keys(HTMLHeadingElement.prototype).map(key => [key, { type: [String, Number, Boolean, Function], default: undefined }])
  )
})

const CardTitle = {
  name: 'CardTitle',
  props: cardTitleProps,
  setup(props, { slots }) {
    return () => (
      <h3
        class={cn('text-2xl font-semibold leading-none tracking-tight', props.class)}
        {...props}
      >
        {slots.default?.()}
      </h3>
    )
  }
}

// CardDescription Component
const cardDescriptionProps = defineProps({
  class: String,
  ...Object.fromEntries(
    Object.keys(HTMLParagraphElement.prototype).map(key => [key, { type: [String, Number, Boolean, Function], default: undefined }])
  )
})

const CardDescription = {
  name: 'CardDescription',
  props: cardDescriptionProps,
  setup(props, { slots }) {
    return () => (
      <p
        class={cn('text-sm text-muted-foreground', props.class)}
        {...props}
      >
        {slots.default?.()}
      </p>
    )
  }
}

// CardContent Component
const cardContentProps = defineProps({
  class: String,
  ...Object.fromEntries(
    Object.keys(HTMLElement.prototype).map(key => [key, { type: [String, Number, Boolean, Function], default: undefined }])
  )
})

const CardContent = {
  name: 'CardContent',
  props: cardContentProps,
  setup(props, { slots }) {
    return () => (
      <div
        class={cn('p-6 pt-0', props.class)}
        {...props}
      >
        {slots.default?.()}
      </div>
    )
  }
}

// CardFooter Component
const cardFooterProps = defineProps({
  class: String,
  ...Object.fromEntries(
    Object.keys(HTMLElement.prototype).map(key => [key, { type: [String, Number, Boolean, Function], default: undefined }])
  )
})

const CardFooter = {
  name: 'CardFooter',
  props: cardFooterProps,
  setup(props, { slots }) {
    return () => (
      <div
        class={cn('flex items-center p-6 pt-0', props.class)}
        {...props}
      >
        {slots.default?.()}
      </div>
    )
  }
}

// Expose sub-components
defineExpose({
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
  CardFooter
})
</script>