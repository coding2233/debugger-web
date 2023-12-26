// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: runtime_debugger_log.proto

#include "runtime_debugger_log.pb.h"

#include <algorithm>

#include <google/protobuf/stubs/common.h>
#include <google/protobuf/io/coded_stream.h>
#include <google/protobuf/extension_set.h>
#include <google/protobuf/wire_format_lite.h>
#include <google/protobuf/descriptor.h>
#include <google/protobuf/generated_message_reflection.h>
#include <google/protobuf/reflection_ops.h>
#include <google/protobuf/wire_format.h>
// @@protoc_insertion_point(includes)
#include <google/protobuf/port_def.inc>
extern PROTOBUF_INTERNAL_EXPORT_runtime_5fdebugger_5flog_2eproto ::PROTOBUF_NAMESPACE_ID::internal::SCCInfo<0> scc_info_LogNode_runtime_5fdebugger_5flog_2eproto;
namespace RuntimeDebugger {
class LogNodeDefaultTypeInternal {
 public:
  ::PROTOBUF_NAMESPACE_ID::internal::ExplicitlyConstructed<LogNode> _instance;
} _LogNode_default_instance_;
class LogListDefaultTypeInternal {
 public:
  ::PROTOBUF_NAMESPACE_ID::internal::ExplicitlyConstructed<LogList> _instance;
} _LogList_default_instance_;
}  // namespace RuntimeDebugger
static void InitDefaultsscc_info_LogList_runtime_5fdebugger_5flog_2eproto() {
  GOOGLE_PROTOBUF_VERIFY_VERSION;

  {
    void* ptr = &::RuntimeDebugger::_LogList_default_instance_;
    new (ptr) ::RuntimeDebugger::LogList();
    ::PROTOBUF_NAMESPACE_ID::internal::OnShutdownDestroyMessage(ptr);
  }
  ::RuntimeDebugger::LogList::InitAsDefaultInstance();
}

::PROTOBUF_NAMESPACE_ID::internal::SCCInfo<1> scc_info_LogList_runtime_5fdebugger_5flog_2eproto =
    {{ATOMIC_VAR_INIT(::PROTOBUF_NAMESPACE_ID::internal::SCCInfoBase::kUninitialized), 1, InitDefaultsscc_info_LogList_runtime_5fdebugger_5flog_2eproto}, {
      &scc_info_LogNode_runtime_5fdebugger_5flog_2eproto.base,}};

static void InitDefaultsscc_info_LogNode_runtime_5fdebugger_5flog_2eproto() {
  GOOGLE_PROTOBUF_VERIFY_VERSION;

  {
    void* ptr = &::RuntimeDebugger::_LogNode_default_instance_;
    new (ptr) ::RuntimeDebugger::LogNode();
    ::PROTOBUF_NAMESPACE_ID::internal::OnShutdownDestroyMessage(ptr);
  }
  ::RuntimeDebugger::LogNode::InitAsDefaultInstance();
}

::PROTOBUF_NAMESPACE_ID::internal::SCCInfo<0> scc_info_LogNode_runtime_5fdebugger_5flog_2eproto =
    {{ATOMIC_VAR_INIT(::PROTOBUF_NAMESPACE_ID::internal::SCCInfoBase::kUninitialized), 0, InitDefaultsscc_info_LogNode_runtime_5fdebugger_5flog_2eproto}, {}};

static ::PROTOBUF_NAMESPACE_ID::Metadata file_level_metadata_runtime_5fdebugger_5flog_2eproto[2];
static const ::PROTOBUF_NAMESPACE_ID::EnumDescriptor* file_level_enum_descriptors_runtime_5fdebugger_5flog_2eproto[1];
static constexpr ::PROTOBUF_NAMESPACE_ID::ServiceDescriptor const** file_level_service_descriptors_runtime_5fdebugger_5flog_2eproto = nullptr;

const ::PROTOBUF_NAMESPACE_ID::uint32 TableStruct_runtime_5fdebugger_5flog_2eproto::offsets[] PROTOBUF_SECTION_VARIABLE(protodesc_cold) = {
  ~0u,  // no _has_bits_
  PROTOBUF_FIELD_OFFSET(::RuntimeDebugger::LogNode, _internal_metadata_),
  ~0u,  // no _extensions_
  ~0u,  // no _oneof_case_
  ~0u,  // no _weak_field_map_
  PROTOBUF_FIELD_OFFSET(::RuntimeDebugger::LogNode, log_time_),
  PROTOBUF_FIELD_OFFSET(::RuntimeDebugger::LogNode, log_frame_count_),
  PROTOBUF_FIELD_OFFSET(::RuntimeDebugger::LogNode, log_type_),
  PROTOBUF_FIELD_OFFSET(::RuntimeDebugger::LogNode, log_message_),
  PROTOBUF_FIELD_OFFSET(::RuntimeDebugger::LogNode, log_stack_track_),
  ~0u,  // no _has_bits_
  PROTOBUF_FIELD_OFFSET(::RuntimeDebugger::LogList, _internal_metadata_),
  ~0u,  // no _extensions_
  ~0u,  // no _oneof_case_
  ~0u,  // no _weak_field_map_
  PROTOBUF_FIELD_OFFSET(::RuntimeDebugger::LogList, log_node_list_),
};
static const ::PROTOBUF_NAMESPACE_ID::internal::MigrationSchema schemas[] PROTOBUF_SECTION_VARIABLE(protodesc_cold) = {
  { 0, -1, sizeof(::RuntimeDebugger::LogNode)},
  { 10, -1, sizeof(::RuntimeDebugger::LogList)},
};

static ::PROTOBUF_NAMESPACE_ID::Message const * const file_default_instances[] = {
  reinterpret_cast<const ::PROTOBUF_NAMESPACE_ID::Message*>(&::RuntimeDebugger::_LogNode_default_instance_),
  reinterpret_cast<const ::PROTOBUF_NAMESPACE_ID::Message*>(&::RuntimeDebugger::_LogList_default_instance_),
};

const char descriptor_table_protodef_runtime_5fdebugger_5flog_2eproto[] PROTOBUF_SECTION_VARIABLE(protodesc_cold) =
  "\n\032runtime_debugger_log.proto\022\017RuntimeDeb"
  "ugger\"\216\001\n\007LogNode\022\020\n\010log_time\030\001 \001(\t\022\027\n\017l"
  "og_frame_count\030\002 \001(\005\022*\n\010log_type\030\003 \001(\0162\030"
  ".RuntimeDebugger.LogType\022\023\n\013log_message\030"
  "\004 \001(\t\022\027\n\017log_stack_track\030\005 \001(\t\":\n\007LogLis"
  "t\022/\n\rlog_node_list\030\001 \003(\0132\030.RuntimeDebugg"
  "er.LogNode*E\n\007LogType\022\t\n\005Error\020\000\022\n\n\006Asse"
  "rt\020\001\022\013\n\007Warning\020\002\022\007\n\003Log\020\003\022\r\n\tException\020"
  "\004b\006proto3"
  ;
static const ::PROTOBUF_NAMESPACE_ID::internal::DescriptorTable*const descriptor_table_runtime_5fdebugger_5flog_2eproto_deps[1] = {
};
static ::PROTOBUF_NAMESPACE_ID::internal::SCCInfoBase*const descriptor_table_runtime_5fdebugger_5flog_2eproto_sccs[2] = {
  &scc_info_LogList_runtime_5fdebugger_5flog_2eproto.base,
  &scc_info_LogNode_runtime_5fdebugger_5flog_2eproto.base,
};
static ::PROTOBUF_NAMESPACE_ID::internal::once_flag descriptor_table_runtime_5fdebugger_5flog_2eproto_once;
static bool descriptor_table_runtime_5fdebugger_5flog_2eproto_initialized = false;
const ::PROTOBUF_NAMESPACE_ID::internal::DescriptorTable descriptor_table_runtime_5fdebugger_5flog_2eproto = {
  &descriptor_table_runtime_5fdebugger_5flog_2eproto_initialized, descriptor_table_protodef_runtime_5fdebugger_5flog_2eproto, "runtime_debugger_log.proto", 329,
  &descriptor_table_runtime_5fdebugger_5flog_2eproto_once, descriptor_table_runtime_5fdebugger_5flog_2eproto_sccs, descriptor_table_runtime_5fdebugger_5flog_2eproto_deps, 2, 0,
  schemas, file_default_instances, TableStruct_runtime_5fdebugger_5flog_2eproto::offsets,
  file_level_metadata_runtime_5fdebugger_5flog_2eproto, 2, file_level_enum_descriptors_runtime_5fdebugger_5flog_2eproto, file_level_service_descriptors_runtime_5fdebugger_5flog_2eproto,
};

// Force running AddDescriptors() at dynamic initialization time.
static bool dynamic_init_dummy_runtime_5fdebugger_5flog_2eproto = (  ::PROTOBUF_NAMESPACE_ID::internal::AddDescriptors(&descriptor_table_runtime_5fdebugger_5flog_2eproto), true);
namespace RuntimeDebugger {
const ::PROTOBUF_NAMESPACE_ID::EnumDescriptor* LogType_descriptor() {
  ::PROTOBUF_NAMESPACE_ID::internal::AssignDescriptors(&descriptor_table_runtime_5fdebugger_5flog_2eproto);
  return file_level_enum_descriptors_runtime_5fdebugger_5flog_2eproto[0];
}
bool LogType_IsValid(int value) {
  switch (value) {
    case 0:
    case 1:
    case 2:
    case 3:
    case 4:
      return true;
    default:
      return false;
  }
}


// ===================================================================

void LogNode::InitAsDefaultInstance() {
}
class LogNode::_Internal {
 public:
};

LogNode::LogNode()
  : ::PROTOBUF_NAMESPACE_ID::Message(), _internal_metadata_(nullptr) {
  SharedCtor();
  // @@protoc_insertion_point(constructor:RuntimeDebugger.LogNode)
}
LogNode::LogNode(const LogNode& from)
  : ::PROTOBUF_NAMESPACE_ID::Message(),
      _internal_metadata_(nullptr) {
  _internal_metadata_.MergeFrom(from._internal_metadata_);
  log_time_.UnsafeSetDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  if (!from.log_time().empty()) {
    log_time_.AssignWithDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), from.log_time_);
  }
  log_message_.UnsafeSetDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  if (!from.log_message().empty()) {
    log_message_.AssignWithDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), from.log_message_);
  }
  log_stack_track_.UnsafeSetDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  if (!from.log_stack_track().empty()) {
    log_stack_track_.AssignWithDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), from.log_stack_track_);
  }
  ::memcpy(&log_frame_count_, &from.log_frame_count_,
    static_cast<size_t>(reinterpret_cast<char*>(&log_type_) -
    reinterpret_cast<char*>(&log_frame_count_)) + sizeof(log_type_));
  // @@protoc_insertion_point(copy_constructor:RuntimeDebugger.LogNode)
}

void LogNode::SharedCtor() {
  ::PROTOBUF_NAMESPACE_ID::internal::InitSCC(&scc_info_LogNode_runtime_5fdebugger_5flog_2eproto.base);
  log_time_.UnsafeSetDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  log_message_.UnsafeSetDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  log_stack_track_.UnsafeSetDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  ::memset(&log_frame_count_, 0, static_cast<size_t>(
      reinterpret_cast<char*>(&log_type_) -
      reinterpret_cast<char*>(&log_frame_count_)) + sizeof(log_type_));
}

LogNode::~LogNode() {
  // @@protoc_insertion_point(destructor:RuntimeDebugger.LogNode)
  SharedDtor();
}

void LogNode::SharedDtor() {
  log_time_.DestroyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  log_message_.DestroyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  log_stack_track_.DestroyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
}

void LogNode::SetCachedSize(int size) const {
  _cached_size_.Set(size);
}
const LogNode& LogNode::default_instance() {
  ::PROTOBUF_NAMESPACE_ID::internal::InitSCC(&::scc_info_LogNode_runtime_5fdebugger_5flog_2eproto.base);
  return *internal_default_instance();
}


void LogNode::Clear() {
// @@protoc_insertion_point(message_clear_start:RuntimeDebugger.LogNode)
  ::PROTOBUF_NAMESPACE_ID::uint32 cached_has_bits = 0;
  // Prevent compiler warnings about cached_has_bits being unused
  (void) cached_has_bits;

  log_time_.ClearToEmptyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  log_message_.ClearToEmptyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  log_stack_track_.ClearToEmptyNoArena(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited());
  ::memset(&log_frame_count_, 0, static_cast<size_t>(
      reinterpret_cast<char*>(&log_type_) -
      reinterpret_cast<char*>(&log_frame_count_)) + sizeof(log_type_));
  _internal_metadata_.Clear();
}

#if GOOGLE_PROTOBUF_ENABLE_EXPERIMENTAL_PARSER
const char* LogNode::_InternalParse(const char* ptr, ::PROTOBUF_NAMESPACE_ID::internal::ParseContext* ctx) {
#define CHK_(x) if (PROTOBUF_PREDICT_FALSE(!(x))) goto failure
  while (!ctx->Done(&ptr)) {
    ::PROTOBUF_NAMESPACE_ID::uint32 tag;
    ptr = ::PROTOBUF_NAMESPACE_ID::internal::ReadTag(ptr, &tag);
    CHK_(ptr);
    switch (tag >> 3) {
      // string log_time = 1;
      case 1:
        if (PROTOBUF_PREDICT_TRUE(static_cast<::PROTOBUF_NAMESPACE_ID::uint8>(tag) == 10)) {
          ptr = ::PROTOBUF_NAMESPACE_ID::internal::InlineGreedyStringParserUTF8(mutable_log_time(), ptr, ctx, "RuntimeDebugger.LogNode.log_time");
          CHK_(ptr);
        } else goto handle_unusual;
        continue;
      // int32 log_frame_count = 2;
      case 2:
        if (PROTOBUF_PREDICT_TRUE(static_cast<::PROTOBUF_NAMESPACE_ID::uint8>(tag) == 16)) {
          log_frame_count_ = ::PROTOBUF_NAMESPACE_ID::internal::ReadVarint(&ptr);
          CHK_(ptr);
        } else goto handle_unusual;
        continue;
      // .RuntimeDebugger.LogType log_type = 3;
      case 3:
        if (PROTOBUF_PREDICT_TRUE(static_cast<::PROTOBUF_NAMESPACE_ID::uint8>(tag) == 24)) {
          ::PROTOBUF_NAMESPACE_ID::uint64 val = ::PROTOBUF_NAMESPACE_ID::internal::ReadVarint(&ptr);
          CHK_(ptr);
          set_log_type(static_cast<::RuntimeDebugger::LogType>(val));
        } else goto handle_unusual;
        continue;
      // string log_message = 4;
      case 4:
        if (PROTOBUF_PREDICT_TRUE(static_cast<::PROTOBUF_NAMESPACE_ID::uint8>(tag) == 34)) {
          ptr = ::PROTOBUF_NAMESPACE_ID::internal::InlineGreedyStringParserUTF8(mutable_log_message(), ptr, ctx, "RuntimeDebugger.LogNode.log_message");
          CHK_(ptr);
        } else goto handle_unusual;
        continue;
      // string log_stack_track = 5;
      case 5:
        if (PROTOBUF_PREDICT_TRUE(static_cast<::PROTOBUF_NAMESPACE_ID::uint8>(tag) == 42)) {
          ptr = ::PROTOBUF_NAMESPACE_ID::internal::InlineGreedyStringParserUTF8(mutable_log_stack_track(), ptr, ctx, "RuntimeDebugger.LogNode.log_stack_track");
          CHK_(ptr);
        } else goto handle_unusual;
        continue;
      default: {
      handle_unusual:
        if ((tag & 7) == 4 || tag == 0) {
          ctx->SetLastTag(tag);
          goto success;
        }
        ptr = UnknownFieldParse(tag, &_internal_metadata_, ptr, ctx);
        CHK_(ptr != nullptr);
        continue;
      }
    }  // switch
  }  // while
success:
  return ptr;
failure:
  ptr = nullptr;
  goto success;
#undef CHK_
}
#else  // GOOGLE_PROTOBUF_ENABLE_EXPERIMENTAL_PARSER
bool LogNode::MergePartialFromCodedStream(
    ::PROTOBUF_NAMESPACE_ID::io::CodedInputStream* input) {
#define DO_(EXPRESSION) if (!PROTOBUF_PREDICT_TRUE(EXPRESSION)) goto failure
  ::PROTOBUF_NAMESPACE_ID::uint32 tag;
  // @@protoc_insertion_point(parse_start:RuntimeDebugger.LogNode)
  for (;;) {
    ::std::pair<::PROTOBUF_NAMESPACE_ID::uint32, bool> p = input->ReadTagWithCutoffNoLastTag(127u);
    tag = p.first;
    if (!p.second) goto handle_unusual;
    switch (::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::GetTagFieldNumber(tag)) {
      // string log_time = 1;
      case 1: {
        if (static_cast< ::PROTOBUF_NAMESPACE_ID::uint8>(tag) == (10 & 0xFF)) {
          DO_(::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::ReadString(
                input, this->mutable_log_time()));
          DO_(::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::VerifyUtf8String(
            this->log_time().data(), static_cast<int>(this->log_time().length()),
            ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::PARSE,
            "RuntimeDebugger.LogNode.log_time"));
        } else {
          goto handle_unusual;
        }
        break;
      }

      // int32 log_frame_count = 2;
      case 2: {
        if (static_cast< ::PROTOBUF_NAMESPACE_ID::uint8>(tag) == (16 & 0xFF)) {

          DO_((::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::ReadPrimitive<
                   ::PROTOBUF_NAMESPACE_ID::int32, ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::TYPE_INT32>(
                 input, &log_frame_count_)));
        } else {
          goto handle_unusual;
        }
        break;
      }

      // .RuntimeDebugger.LogType log_type = 3;
      case 3: {
        if (static_cast< ::PROTOBUF_NAMESPACE_ID::uint8>(tag) == (24 & 0xFF)) {
          int value = 0;
          DO_((::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::ReadPrimitive<
                   int, ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::TYPE_ENUM>(
                 input, &value)));
          set_log_type(static_cast< ::RuntimeDebugger::LogType >(value));
        } else {
          goto handle_unusual;
        }
        break;
      }

      // string log_message = 4;
      case 4: {
        if (static_cast< ::PROTOBUF_NAMESPACE_ID::uint8>(tag) == (34 & 0xFF)) {
          DO_(::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::ReadString(
                input, this->mutable_log_message()));
          DO_(::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::VerifyUtf8String(
            this->log_message().data(), static_cast<int>(this->log_message().length()),
            ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::PARSE,
            "RuntimeDebugger.LogNode.log_message"));
        } else {
          goto handle_unusual;
        }
        break;
      }

      // string log_stack_track = 5;
      case 5: {
        if (static_cast< ::PROTOBUF_NAMESPACE_ID::uint8>(tag) == (42 & 0xFF)) {
          DO_(::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::ReadString(
                input, this->mutable_log_stack_track()));
          DO_(::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::VerifyUtf8String(
            this->log_stack_track().data(), static_cast<int>(this->log_stack_track().length()),
            ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::PARSE,
            "RuntimeDebugger.LogNode.log_stack_track"));
        } else {
          goto handle_unusual;
        }
        break;
      }

      default: {
      handle_unusual:
        if (tag == 0) {
          goto success;
        }
        DO_(::PROTOBUF_NAMESPACE_ID::internal::WireFormat::SkipField(
              input, tag, _internal_metadata_.mutable_unknown_fields()));
        break;
      }
    }
  }
success:
  // @@protoc_insertion_point(parse_success:RuntimeDebugger.LogNode)
  return true;
failure:
  // @@protoc_insertion_point(parse_failure:RuntimeDebugger.LogNode)
  return false;
#undef DO_
}
#endif  // GOOGLE_PROTOBUF_ENABLE_EXPERIMENTAL_PARSER

void LogNode::SerializeWithCachedSizes(
    ::PROTOBUF_NAMESPACE_ID::io::CodedOutputStream* output) const {
  // @@protoc_insertion_point(serialize_start:RuntimeDebugger.LogNode)
  ::PROTOBUF_NAMESPACE_ID::uint32 cached_has_bits = 0;
  (void) cached_has_bits;

  // string log_time = 1;
  if (this->log_time().size() > 0) {
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::VerifyUtf8String(
      this->log_time().data(), static_cast<int>(this->log_time().length()),
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::SERIALIZE,
      "RuntimeDebugger.LogNode.log_time");
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::WriteStringMaybeAliased(
      1, this->log_time(), output);
  }

  // int32 log_frame_count = 2;
  if (this->log_frame_count() != 0) {
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::WriteInt32(2, this->log_frame_count(), output);
  }

  // .RuntimeDebugger.LogType log_type = 3;
  if (this->log_type() != 0) {
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::WriteEnum(
      3, this->log_type(), output);
  }

  // string log_message = 4;
  if (this->log_message().size() > 0) {
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::VerifyUtf8String(
      this->log_message().data(), static_cast<int>(this->log_message().length()),
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::SERIALIZE,
      "RuntimeDebugger.LogNode.log_message");
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::WriteStringMaybeAliased(
      4, this->log_message(), output);
  }

  // string log_stack_track = 5;
  if (this->log_stack_track().size() > 0) {
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::VerifyUtf8String(
      this->log_stack_track().data(), static_cast<int>(this->log_stack_track().length()),
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::SERIALIZE,
      "RuntimeDebugger.LogNode.log_stack_track");
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::WriteStringMaybeAliased(
      5, this->log_stack_track(), output);
  }

  if (_internal_metadata_.have_unknown_fields()) {
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormat::SerializeUnknownFields(
        _internal_metadata_.unknown_fields(), output);
  }
  // @@protoc_insertion_point(serialize_end:RuntimeDebugger.LogNode)
}

::PROTOBUF_NAMESPACE_ID::uint8* LogNode::InternalSerializeWithCachedSizesToArray(
    ::PROTOBUF_NAMESPACE_ID::uint8* target) const {
  // @@protoc_insertion_point(serialize_to_array_start:RuntimeDebugger.LogNode)
  ::PROTOBUF_NAMESPACE_ID::uint32 cached_has_bits = 0;
  (void) cached_has_bits;

  // string log_time = 1;
  if (this->log_time().size() > 0) {
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::VerifyUtf8String(
      this->log_time().data(), static_cast<int>(this->log_time().length()),
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::SERIALIZE,
      "RuntimeDebugger.LogNode.log_time");
    target =
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::WriteStringToArray(
        1, this->log_time(), target);
  }

  // int32 log_frame_count = 2;
  if (this->log_frame_count() != 0) {
    target = ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::WriteInt32ToArray(2, this->log_frame_count(), target);
  }

  // .RuntimeDebugger.LogType log_type = 3;
  if (this->log_type() != 0) {
    target = ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::WriteEnumToArray(
      3, this->log_type(), target);
  }

  // string log_message = 4;
  if (this->log_message().size() > 0) {
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::VerifyUtf8String(
      this->log_message().data(), static_cast<int>(this->log_message().length()),
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::SERIALIZE,
      "RuntimeDebugger.LogNode.log_message");
    target =
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::WriteStringToArray(
        4, this->log_message(), target);
  }

  // string log_stack_track = 5;
  if (this->log_stack_track().size() > 0) {
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::VerifyUtf8String(
      this->log_stack_track().data(), static_cast<int>(this->log_stack_track().length()),
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::SERIALIZE,
      "RuntimeDebugger.LogNode.log_stack_track");
    target =
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::WriteStringToArray(
        5, this->log_stack_track(), target);
  }

  if (_internal_metadata_.have_unknown_fields()) {
    target = ::PROTOBUF_NAMESPACE_ID::internal::WireFormat::SerializeUnknownFieldsToArray(
        _internal_metadata_.unknown_fields(), target);
  }
  // @@protoc_insertion_point(serialize_to_array_end:RuntimeDebugger.LogNode)
  return target;
}

size_t LogNode::ByteSizeLong() const {
// @@protoc_insertion_point(message_byte_size_start:RuntimeDebugger.LogNode)
  size_t total_size = 0;

  if (_internal_metadata_.have_unknown_fields()) {
    total_size +=
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormat::ComputeUnknownFieldsSize(
        _internal_metadata_.unknown_fields());
  }
  ::PROTOBUF_NAMESPACE_ID::uint32 cached_has_bits = 0;
  // Prevent compiler warnings about cached_has_bits being unused
  (void) cached_has_bits;

  // string log_time = 1;
  if (this->log_time().size() > 0) {
    total_size += 1 +
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::StringSize(
        this->log_time());
  }

  // string log_message = 4;
  if (this->log_message().size() > 0) {
    total_size += 1 +
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::StringSize(
        this->log_message());
  }

  // string log_stack_track = 5;
  if (this->log_stack_track().size() > 0) {
    total_size += 1 +
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::StringSize(
        this->log_stack_track());
  }

  // int32 log_frame_count = 2;
  if (this->log_frame_count() != 0) {
    total_size += 1 +
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::Int32Size(
        this->log_frame_count());
  }

  // .RuntimeDebugger.LogType log_type = 3;
  if (this->log_type() != 0) {
    total_size += 1 +
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::EnumSize(this->log_type());
  }

  int cached_size = ::PROTOBUF_NAMESPACE_ID::internal::ToCachedSize(total_size);
  SetCachedSize(cached_size);
  return total_size;
}

void LogNode::MergeFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) {
// @@protoc_insertion_point(generalized_merge_from_start:RuntimeDebugger.LogNode)
  GOOGLE_DCHECK_NE(&from, this);
  const LogNode* source =
      ::PROTOBUF_NAMESPACE_ID::DynamicCastToGenerated<LogNode>(
          &from);
  if (source == nullptr) {
  // @@protoc_insertion_point(generalized_merge_from_cast_fail:RuntimeDebugger.LogNode)
    ::PROTOBUF_NAMESPACE_ID::internal::ReflectionOps::Merge(from, this);
  } else {
  // @@protoc_insertion_point(generalized_merge_from_cast_success:RuntimeDebugger.LogNode)
    MergeFrom(*source);
  }
}

void LogNode::MergeFrom(const LogNode& from) {
// @@protoc_insertion_point(class_specific_merge_from_start:RuntimeDebugger.LogNode)
  GOOGLE_DCHECK_NE(&from, this);
  _internal_metadata_.MergeFrom(from._internal_metadata_);
  ::PROTOBUF_NAMESPACE_ID::uint32 cached_has_bits = 0;
  (void) cached_has_bits;

  if (from.log_time().size() > 0) {

    log_time_.AssignWithDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), from.log_time_);
  }
  if (from.log_message().size() > 0) {

    log_message_.AssignWithDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), from.log_message_);
  }
  if (from.log_stack_track().size() > 0) {

    log_stack_track_.AssignWithDefault(&::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(), from.log_stack_track_);
  }
  if (from.log_frame_count() != 0) {
    set_log_frame_count(from.log_frame_count());
  }
  if (from.log_type() != 0) {
    set_log_type(from.log_type());
  }
}

void LogNode::CopyFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) {
// @@protoc_insertion_point(generalized_copy_from_start:RuntimeDebugger.LogNode)
  if (&from == this) return;
  Clear();
  MergeFrom(from);
}

void LogNode::CopyFrom(const LogNode& from) {
// @@protoc_insertion_point(class_specific_copy_from_start:RuntimeDebugger.LogNode)
  if (&from == this) return;
  Clear();
  MergeFrom(from);
}

bool LogNode::IsInitialized() const {
  return true;
}

void LogNode::InternalSwap(LogNode* other) {
  using std::swap;
  _internal_metadata_.Swap(&other->_internal_metadata_);
  log_time_.Swap(&other->log_time_, &::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(),
    GetArenaNoVirtual());
  log_message_.Swap(&other->log_message_, &::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(),
    GetArenaNoVirtual());
  log_stack_track_.Swap(&other->log_stack_track_, &::PROTOBUF_NAMESPACE_ID::internal::GetEmptyStringAlreadyInited(),
    GetArenaNoVirtual());
  swap(log_frame_count_, other->log_frame_count_);
  swap(log_type_, other->log_type_);
}

::PROTOBUF_NAMESPACE_ID::Metadata LogNode::GetMetadata() const {
  return GetMetadataStatic();
}


// ===================================================================

void LogList::InitAsDefaultInstance() {
}
class LogList::_Internal {
 public:
};

LogList::LogList()
  : ::PROTOBUF_NAMESPACE_ID::Message(), _internal_metadata_(nullptr) {
  SharedCtor();
  // @@protoc_insertion_point(constructor:RuntimeDebugger.LogList)
}
LogList::LogList(const LogList& from)
  : ::PROTOBUF_NAMESPACE_ID::Message(),
      _internal_metadata_(nullptr),
      log_node_list_(from.log_node_list_) {
  _internal_metadata_.MergeFrom(from._internal_metadata_);
  // @@protoc_insertion_point(copy_constructor:RuntimeDebugger.LogList)
}

void LogList::SharedCtor() {
  ::PROTOBUF_NAMESPACE_ID::internal::InitSCC(&scc_info_LogList_runtime_5fdebugger_5flog_2eproto.base);
}

LogList::~LogList() {
  // @@protoc_insertion_point(destructor:RuntimeDebugger.LogList)
  SharedDtor();
}

void LogList::SharedDtor() {
}

void LogList::SetCachedSize(int size) const {
  _cached_size_.Set(size);
}
const LogList& LogList::default_instance() {
  ::PROTOBUF_NAMESPACE_ID::internal::InitSCC(&::scc_info_LogList_runtime_5fdebugger_5flog_2eproto.base);
  return *internal_default_instance();
}


void LogList::Clear() {
// @@protoc_insertion_point(message_clear_start:RuntimeDebugger.LogList)
  ::PROTOBUF_NAMESPACE_ID::uint32 cached_has_bits = 0;
  // Prevent compiler warnings about cached_has_bits being unused
  (void) cached_has_bits;

  log_node_list_.Clear();
  _internal_metadata_.Clear();
}

#if GOOGLE_PROTOBUF_ENABLE_EXPERIMENTAL_PARSER
const char* LogList::_InternalParse(const char* ptr, ::PROTOBUF_NAMESPACE_ID::internal::ParseContext* ctx) {
#define CHK_(x) if (PROTOBUF_PREDICT_FALSE(!(x))) goto failure
  while (!ctx->Done(&ptr)) {
    ::PROTOBUF_NAMESPACE_ID::uint32 tag;
    ptr = ::PROTOBUF_NAMESPACE_ID::internal::ReadTag(ptr, &tag);
    CHK_(ptr);
    switch (tag >> 3) {
      // repeated .RuntimeDebugger.LogNode log_node_list = 1;
      case 1:
        if (PROTOBUF_PREDICT_TRUE(static_cast<::PROTOBUF_NAMESPACE_ID::uint8>(tag) == 10)) {
          ptr -= 1;
          do {
            ptr += 1;
            ptr = ctx->ParseMessage(add_log_node_list(), ptr);
            CHK_(ptr);
            if (!ctx->DataAvailable(ptr)) break;
          } while (::PROTOBUF_NAMESPACE_ID::internal::UnalignedLoad<::PROTOBUF_NAMESPACE_ID::uint8>(ptr) == 10);
        } else goto handle_unusual;
        continue;
      default: {
      handle_unusual:
        if ((tag & 7) == 4 || tag == 0) {
          ctx->SetLastTag(tag);
          goto success;
        }
        ptr = UnknownFieldParse(tag, &_internal_metadata_, ptr, ctx);
        CHK_(ptr != nullptr);
        continue;
      }
    }  // switch
  }  // while
success:
  return ptr;
failure:
  ptr = nullptr;
  goto success;
#undef CHK_
}
#else  // GOOGLE_PROTOBUF_ENABLE_EXPERIMENTAL_PARSER
bool LogList::MergePartialFromCodedStream(
    ::PROTOBUF_NAMESPACE_ID::io::CodedInputStream* input) {
#define DO_(EXPRESSION) if (!PROTOBUF_PREDICT_TRUE(EXPRESSION)) goto failure
  ::PROTOBUF_NAMESPACE_ID::uint32 tag;
  // @@protoc_insertion_point(parse_start:RuntimeDebugger.LogList)
  for (;;) {
    ::std::pair<::PROTOBUF_NAMESPACE_ID::uint32, bool> p = input->ReadTagWithCutoffNoLastTag(127u);
    tag = p.first;
    if (!p.second) goto handle_unusual;
    switch (::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::GetTagFieldNumber(tag)) {
      // repeated .RuntimeDebugger.LogNode log_node_list = 1;
      case 1: {
        if (static_cast< ::PROTOBUF_NAMESPACE_ID::uint8>(tag) == (10 & 0xFF)) {
          DO_(::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::ReadMessage(
                input, add_log_node_list()));
        } else {
          goto handle_unusual;
        }
        break;
      }

      default: {
      handle_unusual:
        if (tag == 0) {
          goto success;
        }
        DO_(::PROTOBUF_NAMESPACE_ID::internal::WireFormat::SkipField(
              input, tag, _internal_metadata_.mutable_unknown_fields()));
        break;
      }
    }
  }
success:
  // @@protoc_insertion_point(parse_success:RuntimeDebugger.LogList)
  return true;
failure:
  // @@protoc_insertion_point(parse_failure:RuntimeDebugger.LogList)
  return false;
#undef DO_
}
#endif  // GOOGLE_PROTOBUF_ENABLE_EXPERIMENTAL_PARSER

void LogList::SerializeWithCachedSizes(
    ::PROTOBUF_NAMESPACE_ID::io::CodedOutputStream* output) const {
  // @@protoc_insertion_point(serialize_start:RuntimeDebugger.LogList)
  ::PROTOBUF_NAMESPACE_ID::uint32 cached_has_bits = 0;
  (void) cached_has_bits;

  // repeated .RuntimeDebugger.LogNode log_node_list = 1;
  for (unsigned int i = 0,
      n = static_cast<unsigned int>(this->log_node_list_size()); i < n; i++) {
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::WriteMessageMaybeToArray(
      1,
      this->log_node_list(static_cast<int>(i)),
      output);
  }

  if (_internal_metadata_.have_unknown_fields()) {
    ::PROTOBUF_NAMESPACE_ID::internal::WireFormat::SerializeUnknownFields(
        _internal_metadata_.unknown_fields(), output);
  }
  // @@protoc_insertion_point(serialize_end:RuntimeDebugger.LogList)
}

::PROTOBUF_NAMESPACE_ID::uint8* LogList::InternalSerializeWithCachedSizesToArray(
    ::PROTOBUF_NAMESPACE_ID::uint8* target) const {
  // @@protoc_insertion_point(serialize_to_array_start:RuntimeDebugger.LogList)
  ::PROTOBUF_NAMESPACE_ID::uint32 cached_has_bits = 0;
  (void) cached_has_bits;

  // repeated .RuntimeDebugger.LogNode log_node_list = 1;
  for (unsigned int i = 0,
      n = static_cast<unsigned int>(this->log_node_list_size()); i < n; i++) {
    target = ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::
      InternalWriteMessageToArray(
        1, this->log_node_list(static_cast<int>(i)), target);
  }

  if (_internal_metadata_.have_unknown_fields()) {
    target = ::PROTOBUF_NAMESPACE_ID::internal::WireFormat::SerializeUnknownFieldsToArray(
        _internal_metadata_.unknown_fields(), target);
  }
  // @@protoc_insertion_point(serialize_to_array_end:RuntimeDebugger.LogList)
  return target;
}

size_t LogList::ByteSizeLong() const {
// @@protoc_insertion_point(message_byte_size_start:RuntimeDebugger.LogList)
  size_t total_size = 0;

  if (_internal_metadata_.have_unknown_fields()) {
    total_size +=
      ::PROTOBUF_NAMESPACE_ID::internal::WireFormat::ComputeUnknownFieldsSize(
        _internal_metadata_.unknown_fields());
  }
  ::PROTOBUF_NAMESPACE_ID::uint32 cached_has_bits = 0;
  // Prevent compiler warnings about cached_has_bits being unused
  (void) cached_has_bits;

  // repeated .RuntimeDebugger.LogNode log_node_list = 1;
  {
    unsigned int count = static_cast<unsigned int>(this->log_node_list_size());
    total_size += 1UL * count;
    for (unsigned int i = 0; i < count; i++) {
      total_size +=
        ::PROTOBUF_NAMESPACE_ID::internal::WireFormatLite::MessageSize(
          this->log_node_list(static_cast<int>(i)));
    }
  }

  int cached_size = ::PROTOBUF_NAMESPACE_ID::internal::ToCachedSize(total_size);
  SetCachedSize(cached_size);
  return total_size;
}

void LogList::MergeFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) {
// @@protoc_insertion_point(generalized_merge_from_start:RuntimeDebugger.LogList)
  GOOGLE_DCHECK_NE(&from, this);
  const LogList* source =
      ::PROTOBUF_NAMESPACE_ID::DynamicCastToGenerated<LogList>(
          &from);
  if (source == nullptr) {
  // @@protoc_insertion_point(generalized_merge_from_cast_fail:RuntimeDebugger.LogList)
    ::PROTOBUF_NAMESPACE_ID::internal::ReflectionOps::Merge(from, this);
  } else {
  // @@protoc_insertion_point(generalized_merge_from_cast_success:RuntimeDebugger.LogList)
    MergeFrom(*source);
  }
}

void LogList::MergeFrom(const LogList& from) {
// @@protoc_insertion_point(class_specific_merge_from_start:RuntimeDebugger.LogList)
  GOOGLE_DCHECK_NE(&from, this);
  _internal_metadata_.MergeFrom(from._internal_metadata_);
  ::PROTOBUF_NAMESPACE_ID::uint32 cached_has_bits = 0;
  (void) cached_has_bits;

  log_node_list_.MergeFrom(from.log_node_list_);
}

void LogList::CopyFrom(const ::PROTOBUF_NAMESPACE_ID::Message& from) {
// @@protoc_insertion_point(generalized_copy_from_start:RuntimeDebugger.LogList)
  if (&from == this) return;
  Clear();
  MergeFrom(from);
}

void LogList::CopyFrom(const LogList& from) {
// @@protoc_insertion_point(class_specific_copy_from_start:RuntimeDebugger.LogList)
  if (&from == this) return;
  Clear();
  MergeFrom(from);
}

bool LogList::IsInitialized() const {
  return true;
}

void LogList::InternalSwap(LogList* other) {
  using std::swap;
  _internal_metadata_.Swap(&other->_internal_metadata_);
  CastToBase(&log_node_list_)->InternalSwap(CastToBase(&other->log_node_list_));
}

::PROTOBUF_NAMESPACE_ID::Metadata LogList::GetMetadata() const {
  return GetMetadataStatic();
}


// @@protoc_insertion_point(namespace_scope)
}  // namespace RuntimeDebugger
PROTOBUF_NAMESPACE_OPEN
template<> PROTOBUF_NOINLINE ::RuntimeDebugger::LogNode* Arena::CreateMaybeMessage< ::RuntimeDebugger::LogNode >(Arena* arena) {
  return Arena::CreateInternal< ::RuntimeDebugger::LogNode >(arena);
}
template<> PROTOBUF_NOINLINE ::RuntimeDebugger::LogList* Arena::CreateMaybeMessage< ::RuntimeDebugger::LogList >(Arena* arena) {
  return Arena::CreateInternal< ::RuntimeDebugger::LogList >(arena);
}
PROTOBUF_NAMESPACE_CLOSE

// @@protoc_insertion_point(global_scope)
#include <google/protobuf/port_undef.inc>
